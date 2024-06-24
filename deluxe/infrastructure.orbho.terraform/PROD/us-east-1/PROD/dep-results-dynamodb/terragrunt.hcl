locals {
  dev_vars        = read_terragrunt_config(find_in_parent_folders("env.hcl"))
  app_environment = local.dev_vars.locals.app_environment
  modules_version = "main"
}

include "root" {
  path = find_in_parent_folders()
}

include "dep-results-dynamodb" {
  path   = "${get_terragrunt_dir()}/../../../../_stacks/dynamodb/dep-results-dynamodb.hcl"
  expose = true
}

terraform {
  source = "${include.dep-results-dynamodb.locals.source_base_url}?ref=${local.modules_version}"
}

inputs = {
  replica_regions                = [{
    region_name            = "us-west-2"
    kms_key_arn            = "arn:aws:kms:us-west-2:851725215854:key/mrk-6252f28de14b424998165c512cd070a7"
    point_in_time_recovery = true
  }]
}
