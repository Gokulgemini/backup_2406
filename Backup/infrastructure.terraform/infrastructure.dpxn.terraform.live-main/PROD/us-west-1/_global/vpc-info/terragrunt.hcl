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
  source = "git::git@github.com:verifyvalid/infrastructure.dpx_shared.terraform.modules.git//vpc-info?ref=${local.modules_version}"
}

# Include all settings from the root terragrunt.hcl file
include {
  path = find_in_parent_folders()
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  bucket_name = local.account_vars.locals.rs_bucket_name
  vpc_remote_state_key = local.account_vars.locals.vpc_remote_state_key
  aws_region = "us-east-1" #overwritting value from us-west-1 to east-1. east-1 is where the tfstate files live
}
