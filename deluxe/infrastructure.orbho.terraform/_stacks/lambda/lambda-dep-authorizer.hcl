locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))
  account_vars     = read_terragrunt_config(find_in_parent_folders("account.hcl"))
  # Extract out common variables for reuse
  infra_environment = lower(replace(local.environment_vars.locals.infra_environment, "_", "-"))
  app_environment   = lower(replace(local.environment_vars.locals.app_environment, "_", "-"))
  landing_zone      = lower(local.account_vars.locals.account_name)
  vpc_info             = local.environment_vars.locals.vpc_info
  source_base_url   = "git::git@bitbucket.org:deluxe-development/aws-lambda.git//"
}
dependency "common-tags" {
  config_path = "${get_terragrunt_dir()}/../overlay-common-tags"
  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy"]
  mock_outputs = {
    common_tags = {
      "AppName" = "dep-authorizer"
    }
  }
}
dependency "vpc-info" {
  config_path = "${get_terragrunt_dir()}/../../_global/vpc-info"
  mock_outputs_allowed_terraform_commands = ["validate", "destroy", "plan-all"]
  mock_outputs = {
    vpc_id = "123"
  }
}
dependency "kms-lambda-artifacts" {
  config_path = "${get_terragrunt_dir()}/../kms-lambda-artifacts"
  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy"]
  mock_outputs = {
    kms_arn           = "arn::"
    kms_id            = "key_id"
    kms_key_alias_arn = "arn::"
  }
}
dependency "s3-lambda-artifacts" {
  config_path = "${get_terragrunt_dir()}/../s3-lambda-artifacts"
  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy"]
  mock_outputs = {
    s3_bucket_id          = "key_id"
    s3_bucket_arn         = "arn::"
    s3_bucket_domain_name = "dsf"
  }
}
dependency "iam-role-lambda-authorizer" {
  config_path = "${get_terragrunt_dir()}/../iam-role-lambda-authorizer"
  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy"]
  mock_outputs = {
    iam_instance_profile_id = "2121"
    iam_role_arn            = "arn::"
    iam_role_name           = "test"
    iam_role_path           = "service"
    iam_role_unique_id      = "12sdasdd"
    role_requires_mfa       = "false"
  }
}
# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  lambdas = [{
    identifier   = "lambda-authorizer"
    name         = "dep-${local.infra_environment}-lambda-authorizer"
    description  = "orbo lambda function"
    handler      = "helloWorldLambda.handler"
    runtime      = "nodejs16.x"
    memory_size  = 512
    timeout      = 300
    iam_role_arn = dependency.iam-role-lambda-authorizer.outputs.iam_role_arn
    kms_key_arn  = dependency.kms-lambda-artifacts.outputs.kms_arn
    s3_existing_package = {
      bucket     = dependency.s3-lambda-artifacts.outputs.s3_bucket_id
      key        = "lambdaauthorizer/helloWorldLambda.zip"
      version_id = null
    }
    vpc_security_group_ids = ["${dependency.vpc-info.outputs.baseline_sg_id}", "${dependency.vpc-info.outputs.app_sg_id}"]
    vpc_subnet_ids         = local.vpc_info.app_subnets
  }]
  tags = merge({ "TerraformPath" = path_relative_to_include() }, dependency.common-tags.outputs.common_tags)
}