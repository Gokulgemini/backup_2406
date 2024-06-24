locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))

  # Extract out common variables for reuse
  parsed_env = replace(local.environment_vars.locals.chef_environment, "_", "-")
  chef_environment = local.environment_vars.locals.chef_environment
  app_environment = local.environment_vars.locals.app_environment
  modules_version = local.environment_vars.locals.modules_version
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "git::git@github.com:verifyvalid/infrastructure.dpx_shared.terraform.modules.git//rds?ref=${local.modules_version}"
}

# Include all settings from the root terragrunt.hcl file
include {
  path = find_in_parent_folders()
}

dependency "kms-db-key" {
  config_path = "${get_terragrunt_dir()}/../kms-db-key"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    kms_arn = "arn::"
    kms_id = "key_id"
    kms_key_alias_arn = "arn::"
  }
}

dependency "common-tags" {
  config_path = "${get_terragrunt_dir()}/../overlay-common-tags"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    common_tags = {
      "AppName" = "DPXN"
    }
  }
}

dependency "vpc-info" {
  config_path = "${get_terragrunt_dir()}/../../_global/vpc-info"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    vpc_id = "123"
    alb_subnets = ["subnet-123","subnet-456"]
    db_prim_subnet_ids = ["subnet-123","subnet-456","subnet-789","subnet-111"]
    app_subnets = ["app-123", "app-456"]
    alb_security_groups = ["sg-1", "sg-2"]
    alb_sg_ids = ["alb-1", "alb-2"]
    app_sg_id = "app-sg-id-1"
    baseline_sg_id = "baseline-sg-id-1"
    db_sg_id = "db-sg-id-1"
    web_sg_id = "web-sg-id-1"
  }
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  db_engine_version = "8.0.33"
  db_engine = "mysql"
  db_instance_class = "db.t3.medium"
  subnet_ids = dependency.vpc-info.outputs.db_prim_subnet_ids
  vpc_security_group_ids = [dependency.vpc-info.outputs.db_sg_id, dependency.vpc-info.outputs.baseline_sg_id]
  multi_az = true
  db_allocated_storage = 100
  app_name = "dpxn"
  performance_insights_enabled = true
  storage_encrypted = true
  deletion_protection = true
  kms_key_id = dependency.kms-db-key.outputs.kms_arn
  performance_insights_kms_key_id = dependency.kms-db-key.outputs.kms_arn
  infra_environment = local.parsed_env
  tags = merge({ "TerraformPath" = path_relative_to_include() }, dependency.common-tags.outputs.common_tags)
}
