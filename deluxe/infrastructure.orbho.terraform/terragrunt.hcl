locals {
  # Automatically load account-level variables
  account_vars = read_terragrunt_config(find_in_parent_folders("account.hcl"))
 
  # Automatically load region-level variables
  region_vars = read_terragrunt_config(find_in_parent_folders("region.hcl"))
 
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))
 
  # Extract the variables we need for easy access
  account_name = local.account_vars.locals.account_name
  account_id   = local.account_vars.locals.aws_account_id
  aws_region   = local.region_vars.locals.aws_region
  tf_statefile_aws_region = local.region_vars.locals.tf_statefile_aws_region
}
 
# Generate an AWS provider block
generate "provider" {
  path      = "provider.tf"
  if_exists = "overwrite_terragrunt"
  contents  = <<EOF
provider "aws" {
  region = "${local.aws_region}"
  # Only these AWS Account IDs may be operated on by this template
  default_tags {
    tags = {
     GitHash = "${run_cmd("bash", "${get_parent_terragrunt_dir()}/scripts/git_data.sh", "hash")}"
     GitBranch = "${run_cmd("bash", "${get_parent_terragrunt_dir()}/scripts/git_data.sh", "branch")}"
   }
  }
 
  allowed_account_ids = ["${local.account_id}"]
  assume_role {
    role_arn = "arn:aws:iam::${local.account_id}:role/Terraform-Admin-Role"
  }
}
EOF
}
 
remote_state {
  backend = "s3"
  config = {
    bucket                = "dep-esa-tf-east"
    key                   = "${get_env("TG_BUCKET_PREFIX", "terraform")}/${path_relative_to_include()}/terraform.tfstate"
    region                = "us-east-1"
    encrypt               = true
    acl                   = "bucket-owner-full-control"
    dynamodb_table        = "terraform-locks"
    role_arn              = "arn:aws:iam::${local.account_id}:role/Terraform-Admin-Role"
    disable_bucket_update = true
  }
  generate = {
    path      = "backend.tf"
    if_exists = "overwrite_terragrunt"
  }
}
 
# ---------------------------------------------------------------------------------------------------------------------
# GLOBAL PARAMETERS
# These variables apply to all configurations in this subfolder. These are automatically merged into the child
# `terragrunt.hcl` config via the include block.
# ---------------------------------------------------------------------------------------------------------------------
 
# Configure root level variables that all resources can inherit. This is especially helpful with multi-account configs
# where terraform_remote_state data sources are placed directly into the modules.
inputs = merge(
  local.account_vars.locals,
  local.region_vars.locals,
  local.environment_vars.locals
)
