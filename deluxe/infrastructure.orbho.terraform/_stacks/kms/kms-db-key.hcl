locals {
  account_vars = read_terragrunt_config(find_in_parent_folders("account.hcl"))
  aws_account_id = local.account_vars.locals.aws_account_id
  region_vars = read_terragrunt_config(find_in_parent_folders("region.hcl"))
  environment_var = read_terragrunt_config(find_in_parent_folders("env.hcl"))
  
  source_base_url = "git::ssh://git@bitbucket.org/deluxe-development/infrastructure.orboanywhereinfra.terraform.stacks.git//kms"
}


inputs = {
  
  aws_account_id = local.aws_account_id
  kms_key_usage = "ENCRYPT_DECRYPT"
  kms_alias = "kms-db-key"
  allow_autoscaling_usage = false
  tags = {
    "TerraformPath" = path_relative_to_include()
  }
}