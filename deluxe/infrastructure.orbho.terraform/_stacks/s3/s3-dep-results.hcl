locals {
  # Automatically load environment-level variables
  account_vars = read_terragrunt_config(find_in_parent_folders("account.hcl"))
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))
  region_vars      = read_terragrunt_config(find_in_parent_folders("region.hcl"))
  environment = lower(local.environment_vars.locals.environment)
  infra_environment = local.environment_vars.locals.infra_environment
  # Extract out common variables for reuse  

  app_environment   = lower(replace(local.environment_vars.locals.app_environment, "_", "-"))
  app_name          = "${local.account_vars.locals.app_name}"
  aws_region        = lower(local.region_vars.locals.aws_region)
  source_base_url   = "git::ssh://git@bitbucket.org/deluxe-development/infrastructure.orboanywhereinfra.terraform.stacks.git//s3-bucket"
}


dependency "common-tags" {
  config_path = "${get_terragrunt_dir()}/../../_global/common-tags"
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
      "AppName" = "${local.account_vars.locals.app_name}"
    }
  }
}

dependency "kms-dep-results" {
  config_path = "${get_terragrunt_dir()}/../kms-dep-results"
  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy", "plan-all"]
  mock_outputs = {
    kms_arn = "arn::"
    kms_id = "key_id"
    kms_key_alias_arn = "arn::"
  }
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {

  bucket_name = "dep-${local.infra_environment}-${local.aws_region}-results" 
  acl = null 
  create_object = true
  key = "backup/"
  lifecycle_rule = [
    {
      id      = "expire_old_backups"
      enabled = true
      prefix  = "backup/"
      tags = {
        Expiration = "true"
      }
      
      expiration = {
        days = 30
      }     
    }
  ]
  encryption_policy = false
  ssl_policy = false
  kms_master_key_id = dependency.kms-dep-results.outputs.kms_arn
  tags = merge(dependency.common-tags.outputs.common_tags, {
    "TerraformPath" = path_relative_to_include()

    
  })
}


