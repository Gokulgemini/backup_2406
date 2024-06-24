locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))
  account_vars     = read_terragrunt_config(find_in_parent_folders("account.hcl"))

  # Extract out common variables for reuse
  infra_environment = lower(replace(local.environment_vars.locals.infra_environment, "_", "-"))
  app_environment   = lower(local.environment_vars.locals.app_environment)
  app_name          = lower(local.account_vars.locals.app_name)
  modules_version   = "1.0.0"
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "git::git@bitbucket.org:deluxe-development/aws-secretsmanager-seed.git//?ref=${local.modules_version}"
}

# Include all settings from the root terragrunt.hcl file
include {
  path = find_in_parent_folders()
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
      "AppName" = "dpxn"
    }
  }
}

dependency "kms-dpxn-secrets-sops" {
  config_path = "${get_terragrunt_dir()}/../kms-dpxn-secrets-sops"

  mock_outputs_allowed_terraform_commands = ["validate", "destroy", "plan-all"]
  mock_outputs = {
    kms_id = "arn:::::123"
  }
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  kms_key_id    = dependency.kms-dpxn-secrets-sops.outputs.kms_id
  secret_prefix = "${local.infra_environment}/${local.app_environment}/${local.app_name}/"
  file_path     = "${get_terragrunt_dir()}/secretfiles/"
  tags = merge(dependency.common-tags.outputs.common_tags, {
    "TerraformPath" = path_relative_to_include()
  })
  recovery_window_in_days = 0
}
