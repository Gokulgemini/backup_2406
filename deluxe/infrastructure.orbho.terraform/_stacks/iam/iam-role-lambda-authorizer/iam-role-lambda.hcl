locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))
  account_vars     = read_terragrunt_config(find_in_parent_folders("account.hcl"))
  region_vars      = read_terragrunt_config(find_in_parent_folders("region.hcl"))

  # Extract out common variables for reuse
  app_environment   = lower(local.environment_vars.locals.app_environment)
  infra_environment = lower(local.account_vars.locals.account_name)
  aws_region        = lower(local.region_vars.locals.aws_region)

  source_base_url   = "git::ssh://git@bitbucket.org/deluxe-development/infrastructure.orboanywhereinfra.terraform.stacks.git//iam"
}

dependency "s3-lambda-artifacts" {
  config_path = "${get_terragrunt_dir()}/../s3-lambda-artifacts"
  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy", "plan-all"]
  mock_outputs = {
    kms_arn = "arn::"
    kms_id = "key_id"
    kms_key_alias_arn = "arn::"
  }
}

dependency "kms-lambda-artifacts" {
  config_path = "${get_terragrunt_dir()}/../kms-lambda-artifacts"
  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy", "plan-all"]
  mock_outputs = {
    kms_arn = "arn::"
    kms_id = "key_id"
    kms_key_alias_arn = "arn::"
  }
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  create_role        = true
  create_policy      = true
  role_name          = "${local.infra_environment}-${local.aws_region}-lambda-artifacts"
  role_description   = "Role used by lambda-artifacts (${local.app_environment})"
  policy_name        = "${local.infra_environment}-${local.aws_region}-lambda-artifacts"
  policy_description = "Policy used by lambda-artifacts (${local.app_environment})"
  role_path          = "/"
  policy_statement   = templatefile("policy/statement.tftpl", { 
    kms-lambda-artifacts-key-arn   = dependency.kms-lambda-artifacts.outputs.kms_arn,
    s3-lambda-artifacts-bucket-arn = dependency.s3-lambda-artifacts.outputs.s3_bucket_arn
  })
  trusted_entities = [
    "lambda.amazonaws.com"
  ]
  tags = {
    "TerraformPath" = path_relative_to_include()
  }
}

