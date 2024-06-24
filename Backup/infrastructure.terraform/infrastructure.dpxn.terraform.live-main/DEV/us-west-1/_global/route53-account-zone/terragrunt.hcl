locals {
  # Automatically load environment-level variables
  account_vars   = read_terragrunt_config(find_in_parent_folders("account.hcl"))
  dns_top_domain = lower("dpxn-${local.account_vars.locals.account_name}")
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "git::ssh://git@bitbucket.deluxe.com:7999/dtf/aws-route53-zone.git//?ref=1.0.1"

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
  }
}

dependency "common-tags" {
  config_path = "${get_terragrunt_dir()}/../common-tags"

  mock_outputs_allowed_terraform_commands = ["validate", "destroy"]
  mock_outputs = {
    asg_common_tags = [
      {
        key                 = "AccountId"
        propagate_at_launch = true
        value               = "3432"
      }
    ]
    common_tags = {
      "AppName" = "DPXN"
    }
  }
}

dependency "vpc-info" {
  config_path = "${get_terragrunt_dir()}/../vpc-info"

  mock_outputs_allowed_terraform_commands = ["validate", "destroy", "plan-all"]
  mock_outputs = {
    vpc_id = "123"
  }
}

# Include all settings from the root terragrunt.hcl file
include {
  path = find_in_parent_folders()
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  dns_name       = "${local.dns_top_domain}.deluxe.com"
  tags           = dependency.common-tags.outputs.common_tags
  vpc_id         = dependency.vpc-info.outputs.vpc_id
  create_private = true
}
