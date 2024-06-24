locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))
  account_vars = read_terragrunt_config(find_in_parent_folders("account.hcl"))
  region_vars = read_terragrunt_config(find_in_parent_folders("region.hcl"))
  modules_version = local.environment_vars.locals.modules_version
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "git::git@github.com:verifyvalid/infrastructure.dpx_shared.terraform.modules.git//kms-key?ref=${local.modules_version}"
}

# Include all settings from the root terragrunt.hcl file
include {
  path = find_in_parent_folders()
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
  is_enabled = true
  enable_key_rotation = true
  app_key_usage = "non_db"
  tags = merge(dependency.common-tags.outputs.common_tags, {
    "TerraformPath" = path_relative_to_include()
  })
}
