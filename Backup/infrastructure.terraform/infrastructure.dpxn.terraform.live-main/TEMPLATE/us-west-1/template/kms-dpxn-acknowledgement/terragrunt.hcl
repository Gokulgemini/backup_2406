locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))
  account_vars = read_terragrunt_config(find_in_parent_folders("account.hcl"))
  region_vars = read_terragrunt_config(find_in_parent_folders("region.hcl"))
  # Extract out common variables for reuse
  app_environment = local.environment_vars.locals.app_environment
  infra_env = local.environment_vars.locals.chef_environment
  modules_version = "3.0.1"

  pci_account_number = local.account_vars.locals.pci_account_number
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "git::git@bitbucket.org:deluxe-development/aws-kms-key.git//?ref=${local.modules_version}"
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

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  is_enabled = true
  enable_key_rotation = true
  kms_alias = "${local.infra_env}-${local.app_environment}-dpxn-acknowledment"
  tags = merge(dependency.common-tags.outputs.common_tags, {
    "TerraformPath" = path_relative_to_include()
  })
}