locals {
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))
  account_vars     = read_terragrunt_config(find_in_parent_folders("account.hcl"))

  infra_environment = lower(replace(local.environment_vars.locals.infra_environment, "_", "-"))
  region_vars       = read_terragrunt_config(find_in_parent_folders("region.hcl"))
  region            = local.region_vars.locals.aws_region
  app_environment   = lower(replace(local.environment_vars.locals.app_environment, "_", "-"))
  app_name          = "DEP"
  source_base_url = "git::ssh://git@bitbucket.org/deluxe-development/infrastructure.orboanywhereinfra.terraform.stacks.git//dynamodb"
}

dependency "common-tags" {
  config_path = "${get_terragrunt_dir()}/../overlay-common-tags"

  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy"]
  mock_outputs = {
    common_tags = {
      "AppName" = "DEP"
    }
  }
}

dependency "kms-db-key" {
  config_path = "${get_terragrunt_dir()}/../../_global/kms-db-key"

  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy"]
  mock_outputs = {
    kms_arn           = "arn::"
    kms_id            = "key_id"
    kms_key_alias_arn = "arn::"
  }
}

inputs = {
  environment                    = local.infra_environment
  region                         = local.region
  server_side_encryption_kms_key_arn = dependency.kms-db-key.outputs.kms_arn
  attributes = [
    {
      name = "PrimaryKey"
      type = "S"
    },
    {
      name: "SortKey",
      type: "S"
    }
  ]
  dynamodb_tags = merge({ "TerraformPath" = path_relative_to_include() }, dependency.common-tags.outputs.common_tags)
}
