locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))

  # Extract out common variables for reuse
  infra_env = replace(local.environment_vars.locals.chef_environment, "_", "-")
  app_environment = local.environment_vars.locals.app_environment

  modules_version = local.environment_vars.locals.dpxn_modules_version
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "git::git@github.com:verifyvalid/infrastructure.dpx_fulfillment.terraform.modules.git//ses-domain?ref=${local.modules_version}"
}

# Include all settings from the root terragrunt.hcl file
include {
  path = find_in_parent_folders()
}


# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  domain  = (local.app_environment == "production" ? "dpxn.echecks.com" : "dpxn-${local.app_environment}.echecks.com")
}
