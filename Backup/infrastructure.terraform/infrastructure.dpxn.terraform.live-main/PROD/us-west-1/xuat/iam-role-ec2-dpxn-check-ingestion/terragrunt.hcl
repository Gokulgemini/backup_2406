locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))
  account_vars = read_terragrunt_config(find_in_parent_folders("account.hcl"))
  account_name = lower(local.account_vars.locals.account_name)

  # Extract out common variables for reuse
  env = local.environment_vars.locals.chef_environment
  app_environment = lower(local.environment_vars.locals.app_environment)
  modules_version = "1.2.4"
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "git::git@bitbucket.org:deluxe-development/aws-iam-role-with-policy.git//?ref=${local.modules_version}"

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


dependency "s3-dpxn-csv-ingestion" {
  config_path = "${get_terragrunt_dir()}/../s3-dpxn-csv-ingestion"

  mock_outputs_allowed_terraform_commands = ["validate","plan"]
  mock_outputs = {
    s3_bucket_id = "key_id"
    s3_bucket_arn = "arn::"
    s3_bucket_domain_name = "dsf"
  }
}

dependency "sqs-dpxn-ingestion-queue" {
  config_path = "${get_terragrunt_dir()}/../sqs-dpxn-ingestion-queue"

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

dependency "kms-non-db-key" {
  config_path = "${get_terragrunt_dir()}/../../_global/kms-non-db-key"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    kms_arn = "arn::"
    kms_id = "key_id"
    kms_key_alias_arn = "arn::"
  }
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

dependency "kms-dpxn-ingestion" {
  config_path = "${get_terragrunt_dir()}/../kms-dpxn-ingestion"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    kms_arn = "arn::"
    kms_id = "key_id"
    kms_key_alias_arn = "arn::"
  }
}

dependency "sns-dpxn-commercial-route-topic" {
  config_path = "${get_terragrunt_dir()}/../sns-dpxn-commercial-route-topic"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    sns_topic_arn = "arn:aws:sns:us-west-1:342434324183:topicName:23e2e27e-7bf0-4b83-9e56-4f6bdb53d2ab"
    sns_topic_id = "topic_id"
    sns_topic_name = "topic_name"
    sns_topic_owner = "topic_owner_account_id"
  }
}

dependency "kms-dpxn-commercial-router" {
  config_path = "${get_terragrunt_dir()}/../kms-dpxn-commercial-router"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    kms_arn = "arn::"
    kms_id = "key_id"
    kms_key_alias_arn = "arn::"
  }
}

dependency "s3-infra-bucket" {
  config_path = "${get_terragrunt_dir()}/../../_global/s3-infra-bucket"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    s3_bucket_id = "key_id"
    s3_bucket_arn = "arn::"
    s3_bucket_domain_name = "dsf"
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
  create_role = true
  create_instance_profile = true
  create_policy = true
  role_name = "check-ingestion-${local.env}-${local.app_environment}-role"
  role_description = "Role used by dpxn check ingestion service (${local.app_environment})"
  policy_name = "check-ingestion-${local.env}-${local.app_environment}-policy"
  policy_description = "Used for the Check Ingestion service (${local.app_environment})"
  policy_statement = templatefile("policy/statement.tftpl", {
    sqs-dpxn-ingestion-queue-arn = dependency.sqs-dpxn-ingestion-queue.outputs.queue_arn,
    s3-infra-bucket-arn = dependency.s3-infra-bucket.outputs.s3_bucket_arn,
    kms-db-key-arn = dependency.kms-db-key.outputs.kms_arn,
    kms-non-db-key-arn = dependency.kms-non-db-key.outputs.kms_arn,
    kms-dpxn-ingestion-arn = dependency.kms-dpxn-ingestion.outputs.kms_arn,
    kms-dpxn-commercial-router = dependency.kms-dpxn-commercial-router.outputs.kms_arn,
    s3-dpxn-csv-ingestion-bucket-arn = dependency.s3-dpxn-csv-ingestion.outputs.s3_bucket_arn,
    sns-dpxn-commercial-route-topic-arn = dependency.sns-dpxn-commercial-route-topic.outputs.sns_topic_arn
  }) 
  attach_managed_policy = true
  managed_policy_arns = [
    "arn:aws:iam::aws:policy/AmazonSSMManagedInstanceCore"
  ]
  trusted_entities = [
    "ec2.amazonaws.com"
  ]
  tags = "${dependency.common-tags.outputs.common_tags}"
  oidc_provider = false
}
