locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))
  account_vars = read_terragrunt_config(find_in_parent_folders("account.hcl"))

  # Extract out common variables for reuse
  app_environment = local.environment_vars.locals.app_environment
  infra_env = replace(local.environment_vars.locals.chef_environment,"_", "-")
  bucket_name = "dpxn-${local.infra_env}-${local.app_environment}-email-inbound"
  modules_version = local.environment_vars.locals.modules_version
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "git::git@github.com:verifyvalid/infrastructure.dpx_shared.terraform.modules.git//s3-bucket?ref=${local.modules_version}"
}

# Include all settings from the root terragrunt.hcl file
include {
  path = find_in_parent_folders()
}

dependency "common-tags" {
  config_path = "${get_terragrunt_dir()}/../overlay-common-tags"

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
      "AppName" = "DPXN"
    }
  }
}

dependency "kms-non-db-key" {
  config_path = "${get_terragrunt_dir()}/../../_global/kms-non-db-key"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    kms_arn = "arn::"
    kms_id = "key_id"
    kms_key_alias_arn = "arn::"
  }
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  bucket_name = local.bucket_name
  acl = "private"
  versioning_enabled = true
  encryption_policy = false
  kms_master_key_id = dependency.kms-non-db-key.outputs.kms_arn
  tags = merge(dependency.common-tags.outputs.common_tags, {
    "TerraformPath" = path_relative_to_include()
    "Name" = local.bucket_name
  })
  additional_statements = [{
      sid = "AllowSESPuts"
      actions = [
        "s3:PutObject"
      ]
      effect = "Allow"
      principals = [{
        type = "Service"
        identifiers = [
          "ses.amazonaws.com"
        ]
      }]
      resources = [
        "arn:aws:s3:::${local.bucket_name}/*"
      ]
      condition = [{
        test = "StringEquals"
        variable = "AWS:SourceAccount"
        values = ["${local.account_vars.locals.aws_account_id}"]
      }]
    }
  ]
}
