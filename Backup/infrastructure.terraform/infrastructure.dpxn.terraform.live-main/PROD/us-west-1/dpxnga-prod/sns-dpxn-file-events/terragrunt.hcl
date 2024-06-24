locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))
  account_vars = read_terragrunt_config(find_in_parent_folders("account.hcl"))
  region_vars = read_terragrunt_config(find_in_parent_folders("region.hcl"))

  # Extract out common variables for reuse
  infra_env = replace(local.environment_vars.locals.chef_environment, "_", "-")
  app_environment = local.environment_vars.locals.app_environment
  topic_name = "${lower(local.infra_env)}-${local.app_environment}-file-events"
  pci_account_number = local.account_vars.locals.pci_account_number
  account_number = local.account_vars.locals.aws_account_id
  aws_region = local.region_vars.locals.aws_region

  modules_version = "1.0.2"
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "git::git@bitbucket.org:deluxe-development/aws-sns.git//?ref=${local.modules_version}"
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

dependency "kms-dpxn-file-events" {
  config_path = "${get_terragrunt_dir()}/../kms-dpxn-file-events"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    kms_arn = "arn::"
    kms_id = "key_id"
    kms_key_alias_arn = "arn::"
  }
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  name = local.topic_name
  sqs_success_feedback_sample_rate = "75"
  kms_master_key_id = dependency.kms-dpxn-file-events.outputs.kms_id
  policy = templatefile("policy/statements.tftpl", {
    pci_account_number     = local.pci_account_number,
    infra_environment      = replace(local.infra_env, "-", "_"),
    app_environment        = local.app_environment,
    file-events-topic-name = local.topic_name,
    account_number         = local.account_number,
    aws_region             = local.aws_region
  })
  tags = merge(dependency.common-tags.outputs.common_tags, {
    "TerraformPath" = path_relative_to_include()
  })
}
