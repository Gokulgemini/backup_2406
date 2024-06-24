# Locally declared variable values
locals {
   # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))
  account_vars = read_terragrunt_config(find_in_parent_folders("account.hcl"))
  region_vars = read_terragrunt_config(find_in_parent_folders("region.hcl"))

  rs_bucket_name = local.account_vars.locals.rs_bucket_name
  vpc_remote_state_key = local.account_vars.locals.vpc_remote_state_key
  aws_region = "us-east-1" # Note that this is the region of the state bucket

  # Define module source
  source_base_url = "git::ssh://git@bitbucket.org/deluxe-development/aws-vpc-info.git//"
}

# Souce terraform module input variables
inputs = {
  bucket_name = local.rs_bucket_name
  vpc_remote_state_key = local.vpc_remote_state_key
  aws_region = local.aws_region
  #get_primary_region_info = true
  get_west_2_region_info = true
}
