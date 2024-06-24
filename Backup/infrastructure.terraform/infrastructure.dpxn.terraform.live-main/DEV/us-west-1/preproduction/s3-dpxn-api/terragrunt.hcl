locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))
  account_vars = read_terragrunt_config(find_in_parent_folders("account.hcl"))

  # Extract out common variables for reuse
  infra_env = replace(local.environment_vars.locals.chef_environment, "_", "-")
  app_environment = local.environment_vars.locals.app_environment
  bucket_name = "dpxn-${lower(local.infra_env)}-${local.app_environment}-dpxn-api"
  modules_version = "2.0.1"

  pci_account_number = local.account_vars.locals.pci_account_number
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "git::ssh://git@bitbucket.deluxe.com:7999/dtf/aws-s3-bucket.git//?ref=${local.modules_version}"
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

dependency "kms-dpxn-api" {
  config_path = "${get_terragrunt_dir()}/../kms-dpxn-api"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    kms_arn = "arn::"
    kms_id = "key_id"
    kms_key_alias_arn = "arn::"
  }
}

dependency "sqs-dpxn-api" {
  config_path = "${get_terragrunt_dir()}/../sqs-dpxn-api"

  mock_outputs_allowed_terraform_commands = ["validate","plan","plan-all"]
  mock_outputs = {
    queue_arn = "arn::"
    queue_id = "12344"
    queue = "queue"
    deadletter_queue_arn = "arn::"
    deadletter_queue_id = "12344"
    deadletter_queue = "d_queue"
  }
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  bucket_name = local.bucket_name
  acl = "private"
  versioning_enabled = true
  encryption_policy = true
  sqs_notifications = {
    lower = {
      events = ["s3:ObjectCreated:*"]
      queue_arn = dependency.sqs-dpxn-api.outputs.queue_arn
      filter_suffix = ".csv"
    },
    upper = {
      events = ["s3:ObjectCreated:*"]
      queue_arn = dependency.sqs-dpxn-api.outputs.queue_arn
      filter_suffix = ".CSV"
    },
  }
  kms_master_key_id = dependency.kms-dpxn-api.outputs.kms_arn
  tags = merge(dependency.common-tags.outputs.common_tags, {
    "TerraformPath" = path_relative_to_include()
    "Name" = local.bucket_name
  })
  additional_statements = [{
    sid = "AllowCrossAccountPreflightAccess"
    actions = [
      "s3:GetObject",
      "s3:GetObjectAcl"
    ]
    effect = "Allow"
    principals = [{
      type = "AWS"
      identifiers = ["arn:aws:iam::${local.pci_account_number}:role/${replace(local.infra_env, "-", "_")}-${local.app_environment}-preflight-role"]
    }]
    resources = [
      "arn:aws:s3:::${local.bucket_name}",
      "arn:aws:s3:::${local.bucket_name}/*"
    ]
    condition = []
  }]
}
