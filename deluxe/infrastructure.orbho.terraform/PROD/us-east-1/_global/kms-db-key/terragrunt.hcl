locals {
  dev_vars        = read_terragrunt_config(find_in_parent_folders("env.hcl"))
  app_environment = local.dev_vars.locals.app_environment
  modules_version = "kms-stack"
}


include "root" {
  path = find_in_parent_folders()
}


include "kms" {
  path   = "${get_terragrunt_dir()}/../../../../_stacks/kms/kms-db-key.hcl"
  expose = true
}


terraform {
  source = "${include.kms.locals.source_base_url}?ref=${local.modules_version}"
}


inputs = {
}