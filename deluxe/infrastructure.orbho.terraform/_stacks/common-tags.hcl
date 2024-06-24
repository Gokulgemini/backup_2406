locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))
  account_vars = read_terragrunt_config(find_in_parent_folders("account.hcl"))
  region_vars = read_terragrunt_config(find_in_parent_folders("region.hcl"))
  
  source_base_url = "git::ssh://git@bitbucket.org:/deluxe-development/aws-common-tags.git//"
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  env_tags = local.environment_vars.locals.env_tags
  account_tags = local.account_vars.locals.account_tags
  region_tags = local.region_vars.locals.region_tags
}
