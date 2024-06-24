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

dependency "kms-dpxn-scan-ingestion" {
  config_path = "${get_terragrunt_dir()}/../kms-dpxn-scan-ingestion"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    kms_arn = "arn::"
    kms_id = "key_id"
    kms_key_alias_arn = "arn::"
  }
}

dependency "eks-fargate-cluster" {
  config_path = "${get_terragrunt_dir()}/../eks-fargate-cluster"
  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    fargate_role_arn = "arn::"
    cluster_name = "cluster"
    oidc_role_url = "oidc.eks.us-west-1.amazonaws.com/id/eeff"
  }
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  oidc_provider           = true
  create_role             = true
  create_policy           = true
  role_name               = "${local.chef_environment}-${local.app_environment}-print-scan-ingestion-role"
  role_description        = "Role used by DPXN Print Scan Ingestion Service (${local.app_environment})"
  policy_name             = "${local.chef_environment}-${local.app_environment}-print-scan-ingestion-policy"
  policy_description      = "Policy used by DPXN Print Scan Ingestion Service Role (${local.app_environment})"
  policy_statement        = templatefile("policy/statements.tftpl", {
    sqs-dpxn-print-scan-ingestion                 = dependency.sqs-dpxn-print-scan-ingestion.outputs.queue_arn,
    s3-dpxn-print-scan-ingestion                  = dependency.s3-dpxn-print-scan-ingestion.outputs.s3_bucket_arn,
    s3-dpxn-print-scan-notifications              = dependency.s3-dpxn-print-scan-notifications.outputs.s3_bucket_arn,
    kms-dpxn-print-scan-ingestion                 = dependency.kms-dpxn-print-scan-ingestion.outputs.kms_arn,
    kms-dpxn-print-scan-notifications             = dependency.kms-dpxn-print-scan-notifications.outputs.kms_arn,
    kms-dpxn-scan-ingestion                        = dependency.kms-dpxn-scan-ingestion.outputs.kms_arn,
    sqs-dpxn-scan-ingestion-nonfifo               = dependency.sqs-dpxn-scan-ingestion-nonfifo.outputs.queue_arn,
    s3-dpxn-scan-ingestion                        = dependency.s3-dpxn-scan-ingestion.outputs.s3_bucket_arn
  })
  oidc_fully_qualified_subjects   = [
    "system:serviceaccount:print-scan-service-layer:dlx-print-scan-ingestion"
  ]
  oidc_fully_qualified_audiences  = [
    "sts.amazonaws.com"
  ]
  provider_url            = dependency.eks-fargate-cluster.outputs.oidc_role_url
  tags                    = {
    "TerraformPath" = path_relative_to_include()
  }
}
