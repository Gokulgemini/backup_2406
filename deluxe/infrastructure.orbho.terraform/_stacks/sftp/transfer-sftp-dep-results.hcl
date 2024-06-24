locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))
  account_vars     = read_terragrunt_config(find_in_parent_folders("account.hcl"))
  region_vars      = read_terragrunt_config(find_in_parent_folders("region.hcl"))

  # Extract out common variables for reuse
  app_environment   = lower(local.environment_vars.locals.app_environment)
  infra_environment = lower(local.account_vars.locals.account_name)

  source_base_url   = "git::ssh://git@bitbucket.org/deluxe-development/aws-sftp.git//"
}

dependency "s3-dep-results" {
  config_path = "${get_terragrunt_dir()}/../s3-dep-results"

  mock_outputs_allowed_terraform_commands = ["validate", "plan"]
  mock_outputs = {
    s3_bucket_id          = "key_id"
    s3_bucket_arn         = "arn::"
    s3_bucket_domain_name = "dsf"
  }
}

dependency "kms-dep-results" {
  config_path = "${get_terragrunt_dir()}/../kms-dep-results"

  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy"]
  mock_outputs = {
    kms_arn           = "arn::"
    kms_id            = "key_id"
    kms_key_alias_arn = "arn::"
  }
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
      "AppName" = "DEP"
    }
  }
}

dependency "vpc-info" {
  config_path = "${get_terragrunt_dir()}/../../_global/vpc-info"

  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy"]
  mock_outputs = {
    vpc_id              = "123"
    alb_subnets         = ["subnet-123", "subnet-456"]
    db_prim_subnet_ids  = ["subnet-123", "subnet-456", "subnet-789", "subnet-111"]
    app_subnets         = ["app-123", "app-456"]
    alb_security_groups = ["sg-1", "sg-2"]
    alb_sg_ids          = ["alb-1", "alb-2"]
    app_sg_id           = "app-sg-id-1"
    baseline_sg_id      = "baseline-sg-id-1"
    db_sg_id            = "db-sg-id-1"
    web_sg_id           = "web-sg-id-1"
  }
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  s3_bucket_id           = dependency.s3-dep-results.outputs.s3_bucket_id
  app_environment        = local.app_environment
  infra_environment      = local.infra_environment
  identity_provider_type = "SERVICE_MANAGED"
  endpoint_type          = "VPC"
  vpc_id                 = dependency.vpc-info.outputs.vpc_id
  subnet_ids             = local.environment_vars.locals.vpc_info.alb_subnets
  vpce_security_group_id = dependency.vpc-info.outputs.baseline_sg_id
  iam_role_name          = "dep-${local.infra_environment}-sftp-results"
  kms_key_arn            = dependency.kms-dep-results.outputs.kms_arn
  tags = merge(dependency.common-tags.outputs.common_tags, {
    "TerraformPath" = path_relative_to_include()
  })
}

