locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))
  account_vars     = read_terragrunt_config(find_in_parent_folders("account.hcl"))

  pci_app_subnet_cidrs = local.account_vars.locals.pci_app_subnet_cidrs

  # Extract out common variables for reuse
  infra_environment = lower(replace(local.environment_vars.locals.infra_environment, "_", "-"))
  app_environment   = lower(local.environment_vars.locals.app_environment)
  app_name          = lower(local.account_vars.locals.app_name)
  modules_version   = "1.0.0"
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "git::ssh://git@bitbucket.deluxe.com:7999/dtf/aws-elasticache-redis-non-clustered.git//?ref=${local.modules_version}"
}

dependency "vpc-info" {
  config_path = "${get_terragrunt_dir()}/../../_global/vpc-info"

  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy", "plan-all"]
  mock_outputs = {
    vpc_id = "123"
  }
}

dependency "subnet-details-db" {
  config_path = "${get_terragrunt_dir()}/../../_global/subnet-details-db"

  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy", "plan-all"]
  mock_outputs = {
    subnet = ["10.0.0.0/24"]
  }
}

dependency "subnet-details-app" {
  config_path = "${get_terragrunt_dir()}/../../_global/subnet-details-app"

  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy", "plan-all"]
  mock_outputs = {
    subnet = ["10.0.1.0/24"]
  }
}

dependency "secrets" {
  config_path = "${get_terragrunt_dir()}/../secrets"

  mock_outputs_allowed_terraform_commands = ["validate", "destroy", "plan-all", "plan"]
  mock_outputs = {
    arns = {
      "${local.app_environment}/redis/authToken" = "arn::::authToken"
    }
  }
}

# Include all settings from the root terragrunt.hcl file
include {
  path = find_in_parent_folders()
}
# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  subnet_ids      = concat(dependency.vpc-info.outputs.db_prim_subnet_ids, dependency.vpc-info.outputs.db_sec_subnet_ids)
  cluster_name    = "${local.infra_environment}-dpxn-redis"
  app_environment = local.app_environment
  node_type       = "cache.t3.small"
  auth_token_arn  = dependency.secrets.outputs.arns["${local.app_environment}/${local.app_name}/backend/redis/authToken"]
  cidr_blocks     = concat(dependency.subnet-details-db.outputs.subnet.*.cidr_block, dependency.subnet-details-app.outputs.subnet.*.cidr_block, local.pci_app_subnet_cidrs.* )
  vpc_id          = dependency.vpc-info.outputs.vpc_id
}
