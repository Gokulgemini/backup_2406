locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))

  # Extract out common variables for reuse
  chef_environment = lower(replace(local.environment_vars.locals.chef_environment, "_", "-"))
  app_environment  = lower(local.environment_vars.locals.app_environment)
  modules_version  = "1.2.4"
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "git::ssh://git@bitbucket.deluxe.com:7999/dtf/aws-iam-role-with-policy.git//?ref=${local.modules_version}"

  extra_arguments "common_var" {
    commands = [
      "apply",
      "apply-all",
      "plan-all",
      "destroy-all",
      "plan",
      "import",
      "push",
      "refresh"
    ]

    arguments = [
      "-var-file=${get_terragrunt_dir()}/${path_relative_from_include()}/common.tfvars",
    ]
  }
}

# Include all settings from the root terragrunt.hcl file
include {
  path = find_in_parent_folders()
}

dependency "s3-dpxn-print-scan-ingestion" {
  config_path = "${get_terragrunt_dir()}/../s3-dpxn-print-scan-ingestion"

  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy"]
  mock_outputs = {
    s3_bucket_id          = "key_id"
    s3_bucket_arn         = "arn::"
    s3_bucket_domain_name = "dsf"
  }
}

dependency "s3-dpxn-print-scan-notifications" {
  config_path = "${get_terragrunt_dir()}/../s3-dpxn-print-scan-notifications"

  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy"]
  mock_outputs = {
    s3_bucket_id          = "key_id"
    s3_bucket_arn         = "arn::"
    s3_bucket_domain_name = "dsf"
  }
}

dependency "sqs-dpxn-print-scan-ingestion" {
  config_path = "${get_terragrunt_dir()}/../sqs-dpxn-print-scan-ingestion"

  mock_outputs_allowed_terraform_commands = ["validate", "plan"]
  mock_outputs = {
    queue_arn            = "arn::"
    queue_id             = "12344"
    queue                = "queue"
    deadletter_queue_arn = "arn::"
    deadletter_queue_id  = "12344"
    deadletter_queue     = "d_queue"
  }
}

dependency "kms-dpxn-print-scan-ingestion" {
  config_path = "${get_terragrunt_dir()}/../kms-dpxn-print-scan-ingestion"

  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy"]
  mock_outputs = {
    kms_arn           = "arn::"
    kms_id            = "key_id"
    kms_key_alias_arn = "arn::"
  }
}

dependency "kms-dpxn-print-scan-notifications" {
  config_path = "${get_terragrunt_dir()}/..//kms-dpxn-print-scan-notifications"

  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy"]
  mock_outputs = {
    kms_arn           = "arn::"
    kms_id            = "key_id"
    kms_key_alias_arn = "arn::"
  }
}

dependency "kms-non-db-key" {
  config_path = "${get_terragrunt_dir()}/../../_global/kms-non-db-key"

  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy"]
  mock_outputs = {
    kms_arn           = "arn::"
    kms_id            = "key_id"
    kms_key_alias_arn = "arn::"
  }
}

dependency "s3-infra-bucket" {
  config_path = "${get_terragrunt_dir()}/../../_global/s3-infra-bucket"

  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy"]
  mock_outputs = {
    s3_bucket_id          = "key_id"
    s3_bucket_arn         = "arn::"
    s3_bucket_domain_name = "dsf"
  }
}

dependency "s3-dpxn-scan-ingestion" {
  config_path = "${get_terragrunt_dir()}/../s3-dpxn-scan-ingestion"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    s3_bucket_id = "key_id"
    s3_bucket_arn = "arn::"
    s3_bucket_domain_name = "dsf"
  }
}

dependency "sqs-dpxn-scan-ingestion-nonfifo" {
  config_path = "${get_terragrunt_dir()}/../sqs-dpxn-scan-ingestion-nonfifo"

  mock_outputs_allowed_terraform_commands = ["validate","plan"]
  mock_outputs = {
    queue_arn = "arn::"
    queue_id = "12344"
    queue = "queue"
    deadletter_queue_arn = "arn::"
    deadletter_queue_id = "12344"
    deadletter_queue = "d_queue"
  }
}

dependency "kms-sqs-scan-ingestion" {
  config_path = "${get_terragrunt_dir()}/../kms-sqs-scan-ingestion"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    kms_arn = "arn::"
    kms_id = "key_id"
    kms_key_alias_arn = "arn::"
  }
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  create_role             = true
  create_instance_profile = true
  create_policy           = true
  role_name               = "${local.chef_environment}-${local.app_environment}-print-scan-ingestion-role"
  role_description        = "Role used by DPXN Print Scan Ingestion Service (${local.app_environment})"
  policy_name             = "${local.chef_environment}-${local.app_environment}-print-scan-ingestion-policy"
  policy_description      = "Policy used by DPXN Print Scan Ingestion Service Role (${local.app_environment})"
  policy_statement        = templatefile("policy/statements.tftpl", {
    sqs-dpxn-print-scan-ingestion                 = dependency.sqs-dpxn-print-scan-ingestion.outputs.queue_arn,
    s3-dpxn-print-scan-ingestion                  = dependency.s3-dpxn-print-scan-ingestion.outputs.s3_bucket_arn,
    s3-dpxn-print-scan-notifications              = dependency.s3-dpxn-print-scan-notifications.outputs.s3_bucket_arn,
    s3-infra-bucket                               = dependency.s3-infra-bucket.outputs.s3_bucket_arn,
    kms-non-db-key                                = dependency.kms-non-db-key.outputs.kms_arn,
    kms-dpxn-print-scan-ingestion                 = dependency.kms-dpxn-print-scan-ingestion.outputs.kms_arn,
    kms-dpxn-print-scan-notifications             = dependency.kms-dpxn-print-scan-notifications.outputs.kms_arn,
    kms-sqs-scan-ingestion                        = dependency.kms-sqs-scan-ingestion.outputs.kms_arn,
    sqs-dpxn-scan-ingestion-nonfifo               = dependency.sqs-dpxn-scan-ingestion-nonfifo.outputs.queue_arn,
    s3-dpxn-scan-ingestion                        = dependency.s3-dpxn-scan-ingestion.outputs.s3_bucket_arn
  })
  attach_managed_policy   = true
  managed_policy_arns = [
    "arn:aws:iam::aws:policy/AmazonSSMManagedInstanceCore"
  ]
  trusted_entities = [
    "ec2.amazonaws.com"
  ]
  oidc_provider = false
}
