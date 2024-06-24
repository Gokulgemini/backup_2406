locals {
  dev_vars        = read_terragrunt_config(find_in_parent_folders("env.hcl"))
  app_environment = local.dev_vars.locals.app_environment
  module_version = "main"
}
# Include all settings from the root terragrunt.hcl file
include "root" {
  path = find_in_parent_folders()
}
# Pull in stack you want.
include "s3-lambda-artifacts" {
  path   = "${get_terragrunt_dir()}/../../../../_stacks/s3/s3-lambda-artifacts.hcl"
  expose = true
}
# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "${include.s3-lambda-artifacts.locals.source_base_url}?ref=${local.module_version}"
}
# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
}