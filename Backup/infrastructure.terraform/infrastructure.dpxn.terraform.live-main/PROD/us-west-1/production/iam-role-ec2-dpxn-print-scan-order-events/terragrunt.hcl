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

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  create_role             = true
  create_instance_profile = true
  create_policy           = true
  role_name               = "${local.chef_environment}-${local.app_environment}-print-scan-order-events-role"
  role_description        = "Role used by DPXN Print Scan Order Events Service (${local.app_environment})"
  policy_name             = "${local.chef_environment}-${local.app_environment}-print-scan-order-events-policy"
  policy_description      = "Policy used by DPXN Print Scan Order Events Service Role (${local.app_environment})"
  policy_statement        = templatefile("policy/statement.tftpl", {
    s3-infra-bucket       = dependency.s3-infra-bucket.outputs.s3_bucket_arn,
    kms-non-db-key        = dependency.kms-non-db-key.outputs.kms_arn
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
