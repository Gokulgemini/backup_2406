locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))

  # Extract out common variables for reuse
  env = local.environment_vars.locals.chef_environment
  app_environment = lower(local.environment_vars.locals.app_environment)
  chef_environment = local.environment_vars.locals.chef_environment
  modules_version = "1.0.2"
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "git::ssh://git@bitbucket.deluxe.com:7999/dtf/aws-security-groups.git//?ref=${local.modules_version}"

  extra_arguments "common_var" {
      commands = [
        "apply",
        "apply-all",
        "plan-all",
        "destroy-all",
        "plan",
        "import",
        "push",
        "refresh"
      ]

      arguments = [
        "-var-file=${get_terragrunt_dir()}/${path_relative_from_include()}/common.tfvars",
      ]
    }

}

# Include all settings from the root terragrunt.hcl file
include {
  path = find_in_parent_folders()
}

dependency "common-tags" {
  config_path = "${get_terragrunt_dir()}/../overlay-common-tags"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    asg_common_tags = [
      {
        key = "AccountId"
        propagate_at_launch = true
        value = "3432"
      }
    ]
    common_tags = {
      "AppName" = "DPXN"
    }
  }
}

dependency "vpc-info" {
  config_path = "${get_terragrunt_dir()}/../../_global/vpc-info"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    vpc_id = "123"
    alb_subnets = ["subnet-123","subnet-456"]
    db_prim_subnet_ids = ["subnet-123","subnet-456","subnet-789","subnet-111"]
    app_subnets = ["app-123", "app-456"]
    alb_security_groups = ["sg-1", "sg-2"]
    alb_sg_ids = ["alb-1", "alb-2"]
    app_sg_id = "app-sg-id-1"
    baseline_sg_id = "baseline-sg-id-1"
    db_sg_id = "db-sg-id-1"
    web_sg_id = "web-sg-id-1"
    web_subnet_ids = ["subnet-123","subnet-456"]
  }
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  security_groups = [{
    identifier =  "cmp-mock"
    sg_name = "${local.env}-${local.app_environment}-cmp-mock"
    sg_description = "This Security Group allows cmp mock api to run"
    vpc_id = dependency.vpc-info.outputs.vpc_id
    ingress_with_cidr_blocks = [{
      from_port   = 3201
      to_port     = 3201
      protocol    = "tcp"
      cidr_blocks = "0.0.0.0/0"
      description = "Opening Inbound Port 3201"
    }]
    egress_with_cidr_blocks = [{
      from_port   = 0
      to_port     = 0
      protocol    = "-1"
      cidr_blocks = "0.0.0.0/0"
      description = "All ports open on outbound"
    }]
    sg_tags = merge(dependency.common-tags.outputs.common_tags, { "TerraformPath" = path_relative_to_include() })
  }]
}
