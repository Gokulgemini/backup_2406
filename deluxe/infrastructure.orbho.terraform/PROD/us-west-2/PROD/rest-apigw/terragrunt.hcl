locals {
  module_version = "api-integrations"
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
  // source = "${include.apigw.locals.source_base_url}?ref=${local.module_version}"
  source = "../../../../../infrastructure.orboanywhereinfra.terraform.stacks/api-gw/"
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  aws_apigw_vpce_sg_id = ["sg-037921d506f354660", "sg-0e569d92e262b0e1b"]
  aws_apigw_vpce_subnets = ["subnet-099210dc098aa0f6b", "subnet-06231754d3ae178cd"]
  aws_vpc_id      = "vpc-0241f6c165bbdc172"
  target_group_ips = ["10.194.148.200", "10.194.148.251"]
}