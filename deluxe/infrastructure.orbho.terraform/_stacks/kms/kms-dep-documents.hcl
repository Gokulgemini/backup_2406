locals {
  # Automatically load environment-level variables
  account_vars = read_terragrunt_config(find_in_parent_folders("account.hcl"))
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))
  environment = lower(local.environment_vars.locals.environment)
  # Extract out common variables for reuse
  infra_environment = local.environment_vars.locals.infra_environment
  source_base_url = "git::ssh://git@bitbucket.org/deluxe-development/infrastructure.orboanywhereinfra.terraform.stacks.git//kms"
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  is_enabled = true
  enable_key_rotation = true
  kms_key_usage = "ENCRYPT_DECRYPT"
  kms_alias = "kms-dep-${local.infra_environment}-documents"
  allow_autoscaling_usage = false
  tags = {
    "TerraformPath" = path_relative_to_include()
  }
}