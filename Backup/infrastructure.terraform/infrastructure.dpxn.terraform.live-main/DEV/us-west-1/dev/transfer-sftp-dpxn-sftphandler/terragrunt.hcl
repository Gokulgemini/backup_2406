locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))

  # Extract out common variables for reuse
  infra_environment = lower(replace(local.environment_vars.locals.infra_environment, "_", "-"))
  app_environment   = lower(local.environment_vars.locals.app_environment)
  modules_version   = "1.0.0"
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

dependency "common-tags" {
  config_path = "${get_terragrunt_dir()}/../overlay-common-tags"

  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy", "plan-all"]
  mock_outputs = {
    asg_common_tags = [
      {
        key                 = "AccountId"
        propagate_at_launch = true
        value               = "3432"
      }
    ]
    common_tags = {
      "AppName" = "sftphandler"
    }
  }
}

dependency "vpc-info" {
  config_path = "${get_terragrunt_dir()}/../../_global/vpc-info"

  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy"]
  mock_outputs = {
    vpc_id         = "123"
    alb_subnets    = ["subnet-123", "subnet-456"]
    baseline_sg_id = "baseline-sg-id-1"
  }
}

dependency "s3-dpxn-sftphandler" {
  config_path = "${get_terragrunt_dir()}/../s3-dpxn-sftphandler"

  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy"]
  mock_outputs = {
    s3_bucket_id          = "key_id"
    s3_bucket_arn         = "arn::"
    s3_bucket_domain_name = "dsf"
  }
}

dependency "kms-dpxn-sftphandler" {
  config_path = "${get_terragrunt_dir()}/../kms-dpxn-sftphandler"

  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy"]
  mock_outputs = {
    kms_arn           = "arn::"
    kms_id            = "key_id"
    kms_key_alias_arn = "arn::"
  }
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  s3_bucket_id           = dependency.s3-dpxn-sftphandler.outputs.s3_bucket_id
  app_environment        = local.app_environment
  infra_environment      = local.infra_environment
  identity_provider_type = "SERVICE_MANAGED"
  vpc_id                 = dependency.vpc-info.outputs.vpc_id
  subnet_ids             = dependency.vpc-info.outputs.alb_subnets
  vpce_security_group_id = dependency.vpc-info.outputs.baseline_sg_id
  endpoint_type          = "VPC"
  iam_role_name          = "${local.infra_environment}-${local.app_environment}-tranfer-sftp-sftphandler"
  kms_key_arn            = dependency.kms-dpxn-sftphandler.outputs.kms_arn
  tags = merge(dependency.common-tags.outputs.common_tags, {
    "TerraformPath" = path_relative_to_include()
  })
}
