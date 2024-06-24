locals {
  module_version = "rest-api-gw"
}

# Include all settings from the root terragrunt.hcl file
include "root" {
  path = find_in_parent_folders()
}

# Pull in stack you want.
include "apigw" {
  path   = "${get_terragrunt_dir()}/../../../../_stacks/api-gw/api-gw-rest.hcl"
  expose = true
}

terraform {
  source = "${include.apigw.locals.source_base_url}?ref=${local.module_version}"
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  aws_apigw_vpce_sg_id = ["sg-0b56a31669a267759"]
  aws_apigw_vpce_subnets = ["subnet-0b7f1de698b38933a", "subnet-072c132b61b577a62"]
  aws_vpc_id      = "vpc-041a1dd0a0a72db5f"
  target_group_ips = ["10.194.152.219", "10.194.152.229"]
}