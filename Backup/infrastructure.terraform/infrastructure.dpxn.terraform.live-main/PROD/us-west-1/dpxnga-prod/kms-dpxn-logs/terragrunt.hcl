locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))
  account_vars = read_terragrunt_config(find_in_parent_folders("account.hcl"))
  region_vars = read_terragrunt_config(find_in_parent_folders("region.hcl"))
  # Extract out common variables for reuse
  app_environment = lower(replace(local.environment_vars.locals.app_environment,"_","-"))
  infra_env = lower(replace(local.environment_vars.locals.chef_environment,"_","-"))
  region = lower(local.region_vars.locals.aws_region)
  aws_account_id = lower(local.account_vars.locals.aws_account_id)

  modules_version = "3.0.1"

  pci_account_number = local.account_vars.locals.pci_account_number
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "git::git@bitbucket.org:deluxe-development/aws-kms-key.git//?ref=${local.modules_version}"
}

# Include all settings from the root terragrunt.hcl file
include {
  path = find_in_parent_folders()
}

dependency "common-tags" {
  config_path = "${get_terragrunt_dir()}/../overlay-common-tags"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
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

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  is_enabled = true
  enable_key_rotation = true
  kms_alias = "${local.infra_env}-${local.app_environment}-dpxn-logs"
  tags = merge(dependency.common-tags.outputs.common_tags, {
    "TerraformPath" = path_relative_to_include()
  })
  additional_policy = [{
    sid = "AllowCloudWatchLogs"
    actions = [
      "kms:Encrypt*",
      "kms:Decrypt*",
      "kms:ReEncrypt*",
      "kms:GenerateDataKey*",
      "kms:Describe*"
    ]
    effect = "Allow"
    principals = [{
      type = "Service"
      identifiers = [
        "logs.${local.region}.amazonaws.com"
      ]
    }]
    resources = [
      "*"
    ]
    condition = [{
      test = "ArnLike",
      variable = "kms:EncryptionContext:aws:logs:arn",
      values = [
        "arn:aws:logs:${local.region}:${local.aws_account_id}:log-group:*"
      ]
    }]
  }]
}
