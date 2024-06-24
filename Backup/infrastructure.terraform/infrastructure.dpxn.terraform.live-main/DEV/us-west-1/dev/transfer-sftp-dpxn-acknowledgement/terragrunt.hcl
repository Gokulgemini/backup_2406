locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))

  # Extract out common variables for reuse
  env = local.environment_vars.locals.chef_environment
  app_env = local.environment_vars.locals.app_environment
  modules_version = "1.0.0.1"
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "git::ssh://git@bitbucket.deluxe.com:7999/dtf/aws-sftp.git//?ref=${local.modules_version}"
}

# Include all settings from the root terragrunt.hcl file
include {
  path = find_in_parent_folders()
}

dependency "s3-dpxn-acknowledgement" {
  config_path = "${get_terragrunt_dir()}/../s3-dpxn-acknowledgement"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    s3_bucket_id = "key_id"
    s3_bucket_arn = "arn::"
    s3_bucket_domain_name = "dsf"
  }
}

dependency "kms-dpxn-acknowledgement" {
  config_path = "${get_terragrunt_dir()}/../kms-dpxn-acknowledgement"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    kms_arn = "arn::"
    kms_id = "key_id"
    kms_key_alias_arn = "arn::"
  }
}

dependency "common-tags" {
  config_path = "${get_terragrunt_dir()}/../overlay-common-tags"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy", "plan-all"]
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

dependency "vpc-info" {
  config_path = "${get_terragrunt_dir()}/../../_global/vpc-info"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    vpc_id = "123"
    alb_subnets = ["subnet-123","subnet-456"]
    db_prim_subnet_ids = ["subnet-123","subnet-456","subnet-789","subnet-111"]
    app_subnets = ["app-123", "app-456"]
    alb_security_groups = ["sg-1", "sg-2"]
    alb_sg_ids = ["alb-1", "alb-2"]
    app_sg_id = "app-sg-id-1"
    baseline_sg_id = "baseline-sg-id-1"
    db_sg_id = "db-sg-id-1"
    web_sg_id = "web-sg-id-1"
  }
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  s3_bucket_id = dependency.s3-dpxn-acknowledgement.outputs.s3_bucket_id
  app_environment = local.app_env
  infra_environment = local.env
  identity_provider_type = "SERVICE_MANAGED"
  vpc_id = dependency.vpc-info.outputs.vpc_id
  subnet_ids = dependency.vpc-info.outputs.alb_subnets
  vpce_security_group_id = dependency.vpc-info.outputs.baseline_sg_id
  endpoint_type = "VPC"
  iam_role_name = "ack"
  kms_key_arn = dependency.kms-dpxn-acknowledgement.outputs.kms_arn
  tags = merge(dependency.common-tags.outputs.common_tags, {
    "TerraformPath" = path_relative_to_include()
  })
}
