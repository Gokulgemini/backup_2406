locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))
  account_vars = read_terragrunt_config(find_in_parent_folders("account.hcl"))
  region_vars = read_terragrunt_config(find_in_parent_folders("region.hcl"))

  # Extract out common variables for reuse
  app_environment = local.environment_vars.locals.app_environment
  infra_env = replace(local.environment_vars.locals.chef_environment,"_", "-")
  queue_name = "dpxn-${local.infra_env}-${local.app_environment}-pcihandoff-queue"
  modules_version = "1.0.0"
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "git::git@bitbucket.org:deluxe-development/aws-sqs.git//?ref=${local.modules_version}"
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

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  name = local.queue_name
  fifo_queue = true
  content_based_deduplication = true
  visibility_timeout_seconds = 600
  delay_seconds = 0
  message_retention_seconds = 1209600
  max_message_size = 262144
  max_receive_count = 1
  kms_master_key_id = dependency.kms-db-key.outputs.kms_arn
  tags = merge(dependency.common-tags.outputs.common_tags, { "TerraformPath" = path_relative_to_include() } )
  policy = [{
    sid = "AllowCrossAccountDeliveryManagerRole"
    actions = [
      "sqs:DeleteMessage",
      "sqs:ReceiveMessage",
      "sqs:SendMessage"
    ]
    effect = "Allow"
    principals = [{
      type = "AWS"
      identifiers = [
        "arn:aws:iam::${local.account_vars.locals.pci_account_number}:role/${replace(local.infra_env,"-","_")}-${local.app_environment}-delivery-manager-role"
      ]
    }]
    resources = [
      "arn:aws:sqs:${local.region_vars.locals.aws_region}:${local.account_vars.locals.aws_account_id}:${local.queue_name}.fifo"
    ]
    condition = []
  }]
}
