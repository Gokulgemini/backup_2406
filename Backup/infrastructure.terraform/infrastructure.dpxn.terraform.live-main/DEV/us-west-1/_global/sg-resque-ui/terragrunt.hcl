locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))
  account_vars = read_terragrunt_config(find_in_parent_folders("account.hcl"))
  region_vars = read_terragrunt_config(find_in_parent_folders("region.hcl"))
  modules_version = "1.3.0"
  env = local.environment_vars.locals.chef_environment
  app_environment = lower(local.environment_vars.locals.app_environment)
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "git::ssh://git@bitbucket.org:/deluxe-development/aws-security-groups//?ref=${local.modules_version}"
}

# Include all settings from the root terragrunt.hcl file
include {
  path = find_in_parent_folders()
}

dependency "vpc-info" {
  config_path = "${get_terragrunt_dir()}/../vpc-info"
  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    vpc_id = "123"
  }
}

dependency "common-tags" {
  config_path = "${get_terragrunt_dir()}/../../_global/common-tags"

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

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  security_groups = [
    {
      identifier =  "resque-ui"
      sg_name = "${local.env}-${local.app_environment}-resque-ui"
      sg_description = "Allow incoming traffic on TCP port 5678"
      vpc_id         = dependency.vpc-info.outputs.vpc_id
      ingress_with_cidr_blocks = [
        {
          from_port   = 5678
          to_port     = 5678
          protocol    = "tcp"
          description = "Allow TCP traffic on port 5678"
          cidr_blocks = "0.0.0.0/0"
        },
        {
          from_port   = 80
          to_port     = 80
          protocol    = "tcp"
          description = "Allow TCP traffic on port 80"
          cidr_blocks = "0.0.0.0/0"
        }
      ]
      egress_with_cidr_blocks = [
        {
          from_port   = 0
          to_port     = 0
          protocol    = "tcp"
          description = "No restrictions on egress"
          cidr_blocks = "0.0.0.0/0"
        }
      ]
      ingress_with_source_security_group_id = []
      sg_tags = merge(dependency.common-tags.outputs.common_tags, { "TerraformPath" = path_relative_to_include() })
    }
  ]
  create_timeout = "15m"
  delete_timeout = "20m"
}
