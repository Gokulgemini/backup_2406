locals {
  # Automatically load environment-level variables
  environment_vars  = read_terragrunt_config(find_in_parent_folders("env.hcl"))
  account_vars      = read_terragrunt_config(find_in_parent_folders("account.hcl"))
  account_name      = lower(local.account_vars.locals.account_name)

  # Extract out common variables for reuse
  env               = local.environment_vars.locals.chef_environment
  app_environment   = lower(local.environment_vars.locals.app_environment)
  modules_version   = "1.2.4"
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

dependency "s3-dpxn-check-order" {
  config_path = "${get_terragrunt_dir()}/../s3-dpxn-check-order"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    s3_bucket_id = "key_id"
    s3_bucket_arn = "arn::"
    s3_bucket_domain_name = "dsf"
  }
}

dependency "sqs-dpxn-check-order" {
  config_path = "${get_terragrunt_dir()}/../sqs-dpxn-check-order"

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

dependency "kms-dpxn-check-order" {
  config_path = "${get_terragrunt_dir()}/../kms-dpxn-check-order"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    kms_arn = "arn::"
    kms_id = "key_id"
    kms_key_alias_arn = "arn::"
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

dependency "s3-infra-bucket" {
  config_path = "${get_terragrunt_dir()}/../../_global/s3-infra-bucket"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    s3_bucket_id = "key_id"
    s3_bucket_arn = "arn::"
    s3_bucket_domain_name = "dsf"
  }
}

dependency "vpc-info" {
  config_path = "${get_terragrunt_dir()}/../../_global/vpc-info"

  mock_outputs_allowed_terraform_commands = ["validate","plan"]
  mock_outputs = {
    cloudwatch_log_group_arn = "arn::"
  }
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  create_role             = true
  create_instance_profile = true
  create_policy           = true
  role_name               = "${local.env}-${local.app_environment}-check-order-role"
  role_description        = "Role used by Check Order (${local.app_environment})"
  policy_name             = "${local.env}-${local.app_environment}-check-order-policy"
  policy_description      = "Used for Check Orders in (${local.app_environment})"
  policy_statement        = templatefile("policy/statements.tftpl", {
                      sqs-dpxn-check-order-queue-arn = dependency.sqs-dpxn-check-order.outputs.queue_arn,
                      s3-dpxn-check-order-bucket-arn = dependency.s3-dpxn-check-order.outputs.s3_bucket_arn,
                      s3-infra-bucket-arn = dependency.s3-infra-bucket.outputs.s3_bucket_arn,
                      kms-non-db-key-arn = dependency.kms-non-db-key.outputs.kms_arn,
                      kms-dpxn-check-order-key-arn = dependency.kms-dpxn-check-order.outputs.kms_arn,
                      logstream-dpxn-check-order-cloudwatch-log-group-arn = dependency.vpc-info.outputs.cloudwatch_log_group_arn

  })
  attach_managed_policy   = true
  managed_policy_arns     = [
    "arn:aws:iam::aws:policy/AmazonSSMManagedInstanceCore"
  ]
  trusted_entities        = [
    "ec2.amazonaws.com"
  ]
  oidc_provider           = false
}
