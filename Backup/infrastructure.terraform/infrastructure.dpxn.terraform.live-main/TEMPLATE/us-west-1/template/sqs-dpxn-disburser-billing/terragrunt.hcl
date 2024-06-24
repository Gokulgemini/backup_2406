locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))

  # Extract out common variables for reuse
  app_environment = lower(local.environment_vars.locals.app_environment)
  infra_environment = lower(replace(local.environment_vars.locals.infra_environment,"_", "-"))
  queue_name = "${local.infra_environment}-${local.app_environment}-disburser-billing"
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

dependency "kms-dpxn-disburser-billing" {
  config_path = "${get_terragrunt_dir()}/../kms-dpxn-disburser-billing"
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
      "AppName" = "disburser-billing"
    }
  }
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  name = local.queue_name
  fifo_queue = false
  content_based_deduplication = false
  visibility_timeout_seconds = 600
  delay_seconds = 0
  message_retention_seconds = 1209600
  max_message_size = 262144
  max_receive_count = 1
  kms_master_key_id = dependency.kms-dpxn-disburser-billing.outputs.kms_arn
  tags = merge(dependency.common-tags.outputs.common_tags, { "TerraformPath" = path_relative_to_include() } )
}