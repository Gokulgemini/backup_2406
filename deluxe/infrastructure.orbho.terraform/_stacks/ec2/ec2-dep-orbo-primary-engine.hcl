locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))
  account_vars     = read_terragrunt_config(find_in_parent_folders("account.hcl"))

  # Extract out common variables for reuse
  vpc_info             = local.environment_vars.locals.vpc_info
  app_environment      = lower(local.environment_vars.locals.app_environment)
  infra_environment    = lower(local.account_vars.locals.account_name)
  app_name             = "dep-${local.infra_environment}-orbo-primary-engine"
  
  source_base_url   = "git::ssh://git@bitbucket.org/deluxe-development/infrastructure.orboanywhereinfra.terraform.stacks.git//ec2"
}

dependency "common-tags" {
  config_path = "${get_terragrunt_dir()}/../overlay-common-tags"

  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy"]
  mock_outputs = {
    asg_common_tags = [
      {
        key                 = "AccountId"
        propagate_at_launch = true
        value               = "3432"
      }
    ]
    common_tags = {
      "AppName" = "${local.account_vars.locals.app_name}"
    }
  }
}

dependency "vpc-info" {
  config_path = "${get_terragrunt_dir()}/../../_global/vpc-info"

  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy"]
  mock_outputs = {
    vpc_id              = "123"
    alb_subnets         = ["subnet-123", "subnet-456"]
    db_prim_subnet_ids  = ["subnet-123", "subnet-456", "subnet-789", "subnet-111"]
    app_subnets         = ["app-123", "app-456"]
    alb_security_groups = ["sg-1", "sg-2"]
    alb_sg_ids          = ["alb-1", "alb-2"]
    app_sg_id           = "app-sg-id-1"
    baseline_sg_id      = "baseline-sg-id-1"
    db_sg_id            = "db-sg-id-1"
    web_sg_id           = "web-sg-id-1"
    web_subnet_ids      = ["subnet-123", "subnet-456"]
  }
}

dependency "key-pair-app" {
  config_path = "${get_terragrunt_dir()}/../../_global/key-pair-app"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    key_pair_fingerprint = "23dfs24"
    key_pair_key_name = "key-pair"
    key_pair_key_pair_id = "1232dfdsdf"
  }
}

dependency "iam-role-dep-orbo-engine" {
  config_path = "${get_terragrunt_dir()}/../iam-role-dep-orbo-engine"

  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy"]
  mock_outputs = {
    iam_policy_policy_arn    = "arn::"
    iam_policy_policy_id     = "12344"
    iam_instance_profile_arn = "arn::"
  }
}

dependency "alb-dep-services" {
  config_path = "${get_terragrunt_dir()}/../alb-dep-services"
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  # Launch Template
  ami_owner_id         = "095876105558"
  ami_filter_string    = "GS-DLX-WIN2022-BASELINE-*"
  launch_template_name        = "${local.app_name}"
  launch_template_description = "${local.app_name} windows instance template"
  instance_type        = "g5.4xlarge"
  directory_id         = "d-9067ffe0f0"
  min_size         = 1
  desired_capacity = 1
  max_size         = 1
  instance_key_name    = dependency.key-pair-app.outputs.key_pair_key_name
  iam_instance_profile = dependency.iam-role-dep-orbo-engine.outputs.iam_instance_profile_id
  instance_security_groups = concat(
    [
      dependency.vpc-info.outputs.baseline_sg_id,
      dependency.vpc-info.outputs.app_sg_id,
      "sg-0c498e75086f3764a"
    ],
    dependency.vpc-info.outputs.alb_sg_ids
  )

  block_device_mappings = [
    {
      # Root volume
      device_name = "/dev/xvda"
      no_device   = 0
      ebs = {
        delete_on_termination = true
        encrypted             = true
        volume_size           = 60
        iops                  = 16000
        volume_type           = "gp3"
      }
    }
  ]

  # Autoscaling Groups
  asg_name         = "${local.app_name}"
  target_group_arns   = [element(dependency.alb-dep-services.outputs.loadbalancer_target_group_arns, 1)]
  vpc_zone_identifier = local.vpc_info.app_subnets

  asg_tags = merge(dependency.common-tags.outputs.common_tags,
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
      value               = "devops-orbo"
      propagate_at_launch = true
    },
    {
      key                 = "ApplicationRole"
      value               = "IIS Webserver"
      propagate_at_launch = true
    },
    {
      key                 = "ComponentName"
      value               = "Orbo-Engine"
      propagate_at_launch = true
    },
    {
      key                 = "Owner"
      value               = "OrboGraph"
      propagate_at_launch = true
    },
    {
      key                 = "OSType"
      value               = "Windows"
      propagate_at_launch = true
    },
    {
      key                 = "OSFlavor"
      value               = "Windows Server 2022"
      propagate_at_launch = true
    },
    {
      key                 = "CICriticality"
      value               = "3 - Low"
      propagate_at_launch = true
    }
  )
}

