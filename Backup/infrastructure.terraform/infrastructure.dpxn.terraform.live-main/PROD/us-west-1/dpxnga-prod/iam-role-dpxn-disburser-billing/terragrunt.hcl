locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))

  # Extract out common variables for reuse
  infra_environment = lower(replace(local.environment_vars.locals.infra_environment,"_", "-"))
  app_environment = lower(local.environment_vars.locals.app_environment)
  modules_version = "1.2.3"
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "git::git@bitbucket.org:deluxe-development/aws-iam-role-with-policy.git//?ref=${local.modules_version}"
}

# Include all settings from the root terragrunt.hcl file
include {
  path = find_in_parent_folders()
}

dependency "s3-dpxn-disburser-billing" {
  config_path = "${get_terragrunt_dir()}/../s3-dpxn-disburser-billing"

  mock_outputs_allowed_terraform_commands = ["validate","plan"]
  mock_outputs = {
    s3_bucket_id = "key_id"
    s3_bucket_arn = "arn::"
    s3_bucket_domain_name = "dsf"
  }
}

dependency "sqs-dpxn-disburser-billing" {
  config_path = "${get_terragrunt_dir()}/../sqs-dpxn-disburser-billing"

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

dependency "kms-dpxn-disburser-billing" {
  config_path = "${get_terragrunt_dir()}/../kms-dpxn-disburser-billing"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    kms_arn = "arn::"
    kms_id = "key_id"
    kms_key_alias_arn = "arn::"
  }
}

dependency "logstream-dpxn-disburser-billing-cloudwatch-log-group" {
  config_path = "${get_terragrunt_dir()}/../../_global/vpc-info"

  mock_outputs_allowed_terraform_commands = ["validate","plan"]
  mock_outputs = {
    cloudwatch_log_group_arn = "arn::"
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
  oidc_provider = true
  create_role = true
  create_policy = true
  role_name = "${local.infra_environment}-${local.app_environment}-disburser-billing-role"
  role_description = "Role used by DLX Disburser Billing Service (${local.app_environment})"
  policy_name = "${local.infra_environment}-${local.app_environment}-disburser-billing-policy"
  policy_description = "Policy used by DLX Disburser Billing Role (${local.app_environment})"
  role_path = "/"
  policy_statement = templatefile("policy/statement.tftpl", {
           sqs-dpxn-disburser-billing-queue-arn = dependency.sqs-dpxn-disburser-billing.outputs.queue_arn,
           s3-dpxn-disburser-billing-bucket-arn = dependency.s3-dpxn-disburser-billing.outputs.s3_bucket_arn,
					 kms-dpxn-disburser-billing-key-arn = dependency.kms-dpxn-disburser-billing.outputs.kms_arn,
					 logstream-dpxn-disburser-billing-cloudwatch-log-group-arn = dependency.logstream-dpxn-disburser-billing-cloudwatch-log-group.outputs.cloudwatch_log_group_arn})
  oidc_fully_qualified_subjects = [
    "system:serviceaccount:reporting:disburser-billing"
  ]
  oidc_fully_qualified_audiences = [
    "sts.amazonaws.com"
  ]
  provider_url = dependency.eks-fargate-cluster.outputs.oidc_role_url

  tags = {
    "TerraformPath" = path_relative_to_include()
  }
}