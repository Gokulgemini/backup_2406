locals {
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))

  # Extract out common variables for reuse
  infra_env = lower(replace(local.environment_vars.locals.chef_environment,"_","-"))
  app_environment = lower(replace(local.environment_vars.locals.app_environment,"_","-"))
  app_name = "dpxn"
  modules_version = "0.1.0"
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "git::git@bitbucket.org:deluxe-development/aws-cloudwatch-group.git//?ref=${local.modules_version}"
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

dependency "kms-dpxn-logs" {
  config_path = "${get_terragrunt_dir()}/../kms-dpxn-logs"
  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    kms_arn = "arn::"
    kms_id = "key_id"
    kms_key_alias_arn = "arn::"
  }
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  name = "${local.infra_env}-${local.app_environment}-${local.app_name}"
  log_retention_in_days = 120
  tags = merge({ "TerraformPath" = path_relative_to_include() }, dependency.common-tags.outputs.common_tags)
  kms_key_id = dependency.kms-dpxn-logs.outputs.kms_arn
}
