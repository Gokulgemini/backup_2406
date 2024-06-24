locals {
  # Automatically load environment-level variables
  environment_vars  = read_terragrunt_config(find_in_parent_folders("env.hcl"))
  account_vars      = read_terragrunt_config(find_in_parent_folders("account.hcl"))
  account_name      = lower(local.account_vars.locals.account_name)

  # Extract out common variables for reuse
  env               = local.environment_vars.locals.chef_environment
  app_environment   = lower(local.environment_vars.locals.app_environment)
  modules_version   = "1.2.4"
  account_id        = local.account_vars.locals.aws_account_id
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

dependency "kms-dpxn-scan-ingestion" {
  config_path = "${get_terragrunt_dir()}/../kms-dpxn-scan-ingestion"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    kms_arn = "arn::"
    kms_id = "key_id"
    kms_key_alias_arn = "arn::"
  }
}

dependency "kms-dpxn-print-scan-ingestion" {
  config_path = "${get_terragrunt_dir()}/../kms-dpxn-print-scan-ingestion"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    kms_arn = "arn::"
    kms_id = "key_id"
    kms_key_alias_arn = "arn::"
  }
}

dependency "kms-dpxn-print-scan-notifications" {
  config_path = "${get_terragrunt_dir()}/../kms-dpxn-print-scan-notifications"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    kms_arn = "arn::"
    kms_id = "key_id"
    kms_key_alias_arn = "arn::"
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

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  create_role             = true
  create_instance_profile = true
  create_policy           = true
  role_name               = "${local.env}-${local.app_environment}-support-toolbox-role"
  role_description        = "Role used by Support Toolbox (${local.app_environment})"
  policy_name             = "${local.env}-${local.app_environment}-support-toolbox-policy"
  policy_description      = "Used for Support toolbox in (${local.app_environment})"
  policy_statement        = templatefile("policy/statements.tftpl", {
                              non_pci_account_number = "${local.account_id}",
                              s3-infra-bucket-arn = dependency.s3-infra-bucket.outputs.s3_bucket_arn,
                              kms-non-db-key-arn = dependency.kms-non-db-key.outputs.kms_arn,
                              kms-dpxn-scan-ingestion-arn = dependency.kms-dpxn-scan-ingestion.outputs.kms_arn,
                              kms-dpxn-print-scan-ingestion-arn = dependency.kms-dpxn-print-scan-ingestion.outputs.kms_arn,
                              kms-dpxn-print-scan-notifications-arn = dependency.kms-dpxn-print-scan-notifications.outputs.kms_arn,
                              kms-dpxn-check-order-arn = dependency.kms-dpxn-check-order.outputs.kms_arn,
                              logstream-dpxn-support-toolbox-cloudwatch-log-group-arn = dependency.vpc-info.outputs.cloudwatch_log_group_arn
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
