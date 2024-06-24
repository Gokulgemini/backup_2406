locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))

  # Extract out common variables for reuse
  env = local.environment_vars.locals.chef_environment
  app_env = local.environment_vars.locals.app_environment
  modules_version = local.environment_vars.locals.dpxn_modules_version
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "git::git@github.com:verifyvalid/infrastructure.dpx_fulfillment.terraform.modules.git//ingestion-lambda?ref=${local.modules_version}"
}

# Include all settings from the root terragrunt.hcl file
include {
  path = find_in_parent_folders()
}

dependency "s3-dpxn-pci-handoff" {
  config_path = "${get_terragrunt_dir()}/../s3-dpxn-pci-handoff"

  mock_outputs_allowed_terraform_commands = ["validate","plan"]
  mock_outputs = {
    s3_bucket_id = "key_id"
    s3_bucket_arn = "arn::"
    s3_bucket_domain_name = "dsf"
  }
}

dependency "sqs-dpxn-pci-handoff" {
  config_path = "${get_terragrunt_dir()}/../sqs-dpxn-pci-handoff"

  mock_outputs_allowed_terraform_commands = ["validate","plan"]
  mock_outputs = {
    queue_arn = "arn::"
    queue_id = "12344"
    queue = "s3-dpxn-pci-handoff"
    deadletter_queue_arn = "arn::"
    deadletter_queue_id = "1243"
    deadletter_queue = "s3-dpxn-pci-handoff-dead"
    queue_url = "https://queue-url"
  }
}

dependency "kms-db-key" {
  config_path = "${get_terragrunt_dir()}/../../_global/kms-db-key"

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
  function_name = "${local.env}-${local.app_env}-dpxn-pci-handoff"
  bucket_name = dependency.s3-dpxn-pci-handoff.outputs.s3_bucket_id
  queue_url = dependency.sqs-dpxn-pci-handoff.outputs.queue_url
  function_filter_suffix = ".json"
  memory_size = 256
  policy_statement = <<EOF
{
  "Version": "2012-10-17",
  "Statement": [
      {
        "Effect": "Allow",
        "Action": "sqs:SendMessage",
        "Resource": "${dependency.sqs-dpxn-pci-handoff.outputs.queue_arn}"
      },{
      "Action": [
        "kms:Encrypt",
        "kms:Decrypt",
        "kms:GenerateDataKey",
        "kms:DescribeKey",
        "kms:ReEncryptFrom",
        "kms:ReEncryptTo"
      ],
      "Effect": "Allow",
      "Resource": "${dependency.kms-db-key.outputs.kms_arn}"
    }
  ]
}
EOF
  tags = merge(dependency.common-tags.outputs.common_tags, { "TerraformPath" = path_relative_to_include() } )
}
