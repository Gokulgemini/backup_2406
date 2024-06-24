# AWS EC2 Instance, Autoscaling Group & Elastic Loadbalancer

This terraform module will be used for creating Autoscaling Groups, Launch Template, Elastic Load Balancer & EC2 instance. Also able to join ec2 instance into domain and control above resources creation using input variables.

## Usage

This module assumes, you creating autoscaling groups with loadbalancer or individual ec2 instance based on your server requirements.

### Using Autoscaling Groups
```bash
locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))
  account_vars     = read_terragrunt_config(find_in_parent_folders("account.hcl"))

  # Extract out common variables for reuse
  app_environment      = lower(local.environment_vars.locals.app_environment)
  app_name             = "mpc-payroll"
  certificate_arn      = local.account_vars.locals.wildcard_arn
  iam_instance_profile = "EC2_IAM_Role"
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "git::ssh://git@bitbucket.deluxe.com:7999/dtf/aws-ec2-asg-alb.git//?ref=1.9.0"
}

dependency "common-tags" {
  config_path = "${get_terragrunt_dir()}/../common-tags"
}

dependency "vpc-info" {
  config_path = "${get_terragrunt_dir()}/../../_global/vpc-info"
}

dependency "common-sg" {
  config_path = "${get_terragrunt_dir()}/../../_global/common-security-groups"
}

# Include all settings from the root terragrunt.hcl file
include {
  path = find_in_parent_folders()
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  # Load Balancers & Target Groups
  create_alb = true
  alb_name    = "${local.app_environment}-${local.app_name}"
  vpc_id      = dependency.vpc-info.outputs.vpc_id
  alb_subnets = dependency.vpc-info.outputs.alb_subnets
  alb_security_groups = concat(
    [
      dependency.vpc-info.outputs.baseline_sg_id,
      dependency.vpc-info.outputs.web_sg_id
    ],
    dependency.vpc-info.outputs.alb_sg_ids
  )
  alb_target_groups = [{
    "name"             = "${local.app_environment}-${local.app_name}-https",
    "backend_protocol" = "HTTPS",
    "backend_port"     = 443,
    "target_type"      = "instance"
    health_check = {
      enabled             = true
      path                = "/"
      port                = 443
      healthy_threshold   = 5
      unhealthy_threshold = 5
      timeout             = 5
      interval            = 30
      protocol            = "HTTPS"
      matcher             = "200"
    }
  }]

  http_tcp_listeners = [{
    port               = 80
    protocol           = "HTTP"
    target_group_index = 0
  }]

  https_listeners = [{
    port               = 443
    protocol           = "HTTPS"
    certificate_arn    = local.certificate_arn
    target_group_index = 0
  }]
  alb_access_logs = {
    enabled = true
    bucket  = "<id-of-bucket-which-will-store-lb-logs>"
    prefix  = "<directory-prefix-to-use-in-bucket-for-this-lb-logs>"
  }
  alb_tags = merge({ "TerraformPath" = path_relative_to_include() }, dependency.common-tags.outputs.common_tags)

  # Launch Configuration/Template

  create_lc            = false
  lc_name              = "${local.app_environment}-${local.app_name}"

  create_lt            = true
  launch_template_name = "${local.app_environment}-${local.app_name}"
  launch_template_description = "App ec2 server windows"

  ami_owner_id         = "095876105558"
  ami_filter_string    = "GS-DLX-WIN-BASELINE-*"
  instance_type        = "t3a.large"
  iam_instance_profile = local.iam_instance_profile
  enable_domain_join   = true
  directory_id         = "d-91673df896"

  instance_security_groups = concat(
    [
      dependency.vpc-info.outputs.baseline_sg_id,
      dependency.vpc-info.outputs.web_sg_id,
      dependency.common-sg.outputs.sg_ids["ansible-sg"].security_group_id,
      dependency.common-sg.outputs.sg_ids["snow-discovery-sg"].security_group_id
    ],
    dependency.vpc-info.outputs.alb_sg_ids
  )

  root_block_device = [{
    delete_on_termination = true
    volume_size           = 100
    volume_type           = "gp2"
  },]

  ebs_block_device = [{
    device_name           = "xvdb"
    delete_on_termination = true
    volume_size           = 100
    volume_type           = "gp2"
    encrypted             = true
    },{
    device_name           = "xvdc"
    delete_on_termination = true
    volume_size           = 25
    volume_type           = "gp2"
    encrypted             = true
  },]

  block_device_mappings = [
    {
      # Root volume
      device_name = "/dev/xvda"
      no_device   = 0
      ebs = {
        delete_on_termination = true
        encrypted             = true
        volume_size           = 20
        volume_type           = "gp2"
      }
      }, {
      device_name = "/dev/xvdb"
      no_device   = 1
      ebs = {
        delete_on_termination = true
        encrypted             = true
        volume_size           = 30
        volume_type           = "gp2"
      }
    }
  ]

  network_interfaces = [
    {
      delete_on_termination = true
      description           = "eth0"
      device_index          = 0
      security_groups       = [dependency.vpc-info.outputs.baseline_sg_id, dependency.vpc-info.outputs.web_sg_id]
    },
    {
      delete_on_termination = true
      description           = "eth1"
      device_index          = 1
      security_groups       = [dependency.vpc-info.outputs.baseline_sg_id, dependency.vpc-info.outputs.web_sg_id]
    }
  ]

  monitoring = true

  # Autoscaling Groups
  create_ec2          = false
  create_asg          = true

  use_lc              = false
  use_lt              = true

  asg_name            = "${local.app_environment}-${local.app_name}"
  min_size            = 1
  max_size            = 1
  desired_capacity    = 1
  vpc_zone_identifier = dependency.vpc-info.outputs.web_subnet_ids

  asg_tags = concat(dependency.common-tags.outputs.asg_common_tags,
    [
      {
        key                 = "TerraformPath"
        value               = path_relative_to_include()
        propagate_at_launch = true
      },
      {
        key                 = "Domain"
        value               = "deluxe.com"
        propagate_at_launch = true
      },
      {
        key                 = "Application"
        value               = "MPC"
        propagate_at_launch = true
      },
      {
        key                 = "ApplicationRole"
        value               = "IIS Webserver"
        propagate_at_launch = true
      },
      {
        key                 = "ComponentName"
        value               = "Payroll"
        propagate_at_launch = true
      },
      {
        key                 = "Owner"
        value               = "MyPayCenter"
        propagate_at_launch = true
      },
      {
        key                 = "OSType"
        value               = "Windows"
        propagate_at_launch = true
      },
      {
        key                 = "OSFlavor"
        value               = "Windows 2019"
        propagate_at_launch = true
      },
      {
        key                 = "CICriticality"
        value               = "3 - Low"
        propagate_at_launch = true
      },
      {
        key                 = "ManagedBy"
        value               = "Mike Crist, Aruna Madiraju"
        propagate_at_launch = true
      }
  ])
}
```

### Using EC2 Instance Only
```bash
locals {
  # Automatically load environment-level variables
  account_vars = read_terragrunt_config(find_in_parent_folders("account.hcl"))
  landing_zone = lower(local.account_vars.locals.account_name)
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
 source = "git::ssh://git@bitbucket.deluxe.com:7999/dtf/aws-ec2-asg-alb//?ref=2.0.0"
}

dependency "common-tags" {
  config_path = "${get_terragrunt_dir()}/../common-tags"
}

dependency "vpc-info" {
  config_path = "${get_terragrunt_dir()}/../vpc-info"
}

# Include all settings from the root terragrunt.hcl file
include {
  path = find_in_parent_folders()
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  create_ec2 = true
  create_lb = false
  create_lc = false
  create_lt = false
  create_asg = false

  instance_name = "jenkins-node-${local.landing_zone}-dr"
  instance_type = "t3.large"
  instance_key_name = "mypaycenter-${local.landing_zone}-dr"
  iam_instance_profile = "EC2_TerraformAdmin_Profile"

  ami_owner_id         = "095876105558"
  ami_filter_string    = "Template-RedHat7-Packer"

  subnet_id = element(dependency.vpc-info.outputs.web_subnet_ids, 0)
  instance_security_groups = [
    dependency.vpc-info.outputs.baseline_sg_id,
    dependency.vpc-info.outputs.web_sg_id
  ]

  root_block_device = [{
    delete_on_termination = true
    volume_size           = 200
    volume_type           = "gp2"
  },]

  monitoring = true

  instance_tags = merge(dependency.common-tags.outputs.common_tags, 
  {
    TerraformPath = path_relative_to_include()
    Owner = "MyPayCenter"
    OSType = "Linux"
    OSFlavor = "RedHat Enterprise Linux 7"
    CICriticality = "3 - Low"
    ManagedBy = "Mike Crist, Aruna Madiraju"
  })
}
```

## Input Variables

| Variable Name | Description | Default Value | Usage Notes |
|--|--|--|--|
| create_lb | Determines whether to create loadbalancer or not. | true | This will opt is to create elastic loadbalancer |
| create_lc | Determines whether to create ec2 launch configuration or not. | true | This will opt is to create ec2 launch configuration |
| create_lt | Determines whether to create ec2 launch template or not. | false | This will opt is to create ec2 launch template |
| use_lc | Determines whether to use ec2 launch configuration to create autoscaling groups. | true | This will opt is to create ec2 autoscaling groups using launch configuration |
| use_lt | Determines whether to use ec2 launch template to create autoscaling groups. | false | This will opt is to create ec2 autoscaling groups using launch template |
| create_asg | Determines whether to create ec2 autosacling groups or not. | true | This will opt is to create ec2 autosacling groups |
| create_ec2 | Determines whether to create ec2 instance or not. | false | This will opt is to create ec2 instance only |
| create_keypair | Determines whether to create ec2 keypair or not. | false | This will opt is to create ec2 keypair |
| alb_name | The resource name and Name tag of the load balancer. | alb | This will be used for elastic loadbalancer name if create_lb is true |
| lc_name | Name of launch configuration to be created. | lc | This will be used for launch configuration name if create_lc is true |
| launch_template_name | Name of launch template to be created. | lt | This will be used for launch template name if create_lt is true |
| launch_template_description | Short description about launch template to be created. | null | This will be describe about launch template name if create_lt is true |
| launch_template_version | Launch template default version. | null | This will be set default version value if create_lt is true |
| asg_name | Name used for autoscaling groups. | asg | This will be used for autoscaling group name if create_asg is true |
| instance_name | The resource name and Name tag for ec2 instance. | "" | This will be used for ec2 instance tag name if create_ec2 is true |
| load_balancer_type | The Type of elastic loadbalancer. | application | This will be used for choosing elb type |
| vpc_id | The unique ID of the VPC | "" | This will be used for choosing network for ec2 resource |
| alb_subnets | Array of the Subnets we should be using with ALB | [] | This will be used for selecting subnets for elb |
| alb_security_groups | Array of security groups used for the load balancer | [] | This will be used for selecting security groups for elb |
| alb_target_groups | The target groups configuration | [] | This will be used for selecting target groups for alb or nlb |
| http_tcp_listeners | http_tcp_listeners configuration | [] | This will be used for adding tcp http lister for alb or nlb |
| https_listeners | https_tcp_listeners configuration | [] | This will be used for adding tcp https lister for alb or nlb |
| https_listener_rules | https_tcp_listeners rules | [] | This will be used for adding tcp https lister rules for alb or nlb |
| http_tcp_listener_rules | http_tcp_listeners rules | [] | This will be used for adding tcp http lister rules for alb or nlb |
| ebs_optimized | Whether we should enable ebs_optimized | true | This will opt is to enable ebs_optimized or not |
| ami_id | Image Id for ec2 instance creation | "" | This will be used for ec2 instance creation |
| ami_owner_id | Owner id for the AMI | "" | This will be used for filtering ami using owner id |
| ami_filter_string | Filter string for finding suitable AMI | "" | This will be used for filtering ami using regex string |
| instance_type | EC2 Instance Type | "t4g.micro" | This will be used for choosing instance type |
| iam_instance_profile | EC2 Instance IAM profile or role | "" | This will be used for attaching IAM role or profile to ec2 instance |
| instance_key_name | EC2 Instance keypair name | "" | This will be used for attaching key pair to ec2 instance while creation |
| user_data_base64 | The Base64-encoded user data to provide when launching the instance | "" | This will be used for passing encoded format of userdata script |
| user_data_replace_on_change | When used in combination with user_data or user_data_base64 will trigger a destroy and recreate when set to true | false | This will be used for replacing instance based on userdata script changes |
| instance_security_groups | Array of security groups used for the ec2 instance or autoscaling groups | [] | This will be used for attaching security groups to ec2 instance or asg |
| associate_public_ip_address | Associate a public ip address with an instance in a VPC | false | This will be used for associating public ip to ec2 instance |
| ebs_block_device | Additional EBS block devices to attach to the instance | [] | This will be used for attaching extra ebs volume to ec2 instance |
| root_block_device | Customize details about the root block device of the instance | [] | This will be used for resizing root volume in or from ec2 instance |
| block_device_mappings | Customize details about the attachment of block device to autoscaling instances | [] | This will be used for configuring ebs root/additional volume on ec2 instance during provision |
| network_interfaces | Customize network interfaces to be attached at instance boot time | [] | This will be used for configuring n/w interface cards on ec2 instance during provision |
| max_size | Max Size for AutoScaling Group | 1 | This will be used for provide max count for asg instances |
| min_size | Min Size for AutoScaling Group | 1 | This will be used for provide min count for asg instances |
| desired_capacity | Desired Capacity for AutoScaling Group | 1 | This will be used for provide must available count for asg instances |
| default_cooldown | Default Cooldown for AutoScaling Group | 300 | This specifies how long any alarm-triggered scaling action will be disallowed after a previous scaling action is complete |
| health_check_type | Type of Health Check | EC2 | This specifies type of health check for asg |
| subnet_id | The VPC Subnet ID for ec2 instance | "" | This specifies subnet id for ec2 instance |
| vpc_zone_identifier | Array of subnets for asg instances | "" | This specifies subnet ids for asg instances |
| enable_domain_join | Whether user data config is required for ec2 instances | false | This will be used to enable domain join or not |
| directory_id | AWS directory service id to join the domain with file system | "" | This will be used for Managed Microsoft AD instead of Self Managed AD |
| windows_version | Names the version of Windows server used in the userdata Powershell script | "CloudCommon2019" | This will be used to manage the version of Windows used in the userdata script |
| monitoring | If true, the launched EC2 instance will have detailed monitoring enabled | true | This will enable detailed monitoring for ec2 instance |
| instance_tags | A mapping of tags to assign to instance at launch time | {} | This will add resource tags for ec2 instance |
| volume_tags | A mapping of tags to assign to the devices created by the instance at launch time | {} | This will add resource tags for ebs volumes |
| enable_volume_tags | Whether to enable volume tags (if enabled it conflicts with root_block_device tags) | false | This will enable volume tags or not |
| alb_tags | A map of tags to add to loadbalancer and target groups | {} | This will be used for adding resource tags for alb or nlb |
| asg_tags | A map of tags to add to auto scaling groups | {} | This will be used for adding resource tags for asg resources |
| timeouts | The timeout settings for create,update and delete lifecycle | {} | This will be used to change terraform api timeout for create, delete, update action |
---
