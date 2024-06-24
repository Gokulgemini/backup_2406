locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))

  # Extract out common variables for reuse
  infra_environment = lower(replace(local.environment_vars.locals.infra_environment, "_", "-"))
  app_environment   = lower(local.environment_vars.locals.app_environment)
  modules_version   = "1.2.3"
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

dependency "logstream-dpxn-payment-reporting-api-cloudwatch-log-group" {
  config_path = "${get_terragrunt_dir()}/../../_global/vpc-info"

  mock_outputs_allowed_terraform_commands = ["validate", "plan"]
  mock_outputs = {
    cloudwatch_log_group_arn = "arn::"
  }
}

dependency "eks-fargate-cluster" {
  config_path = "${get_terragrunt_dir()}/../eks-fargate-cluster"

  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy"]
  mock_outputs = {
    fargate_role_arn = "arn::"
    cluster_name     = "cluster"
    oidc_role_url    = "oidc.eks.us-west-1.amazonaws.com/id/eeff"
  }
}

dependency "kms-dpxn-payment-reporting" {
  config_path                             = "${get_terragrunt_dir()}/../kms-dpxn-payment-reporting"
  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy"]
  mock_outputs = {
    kms_arn           = "arn::"
    kms_id            = "key_id"
    kms_key_alias_arn = "arn::"
  }
}

dependency "s3-dpxn-userreportdata" {
  config_path                             = "${get_terragrunt_dir()}/../s3-dpxn-userreportdata"
  mock_outputs_allowed_terraform_commands = ["validate", "plan"]
  mock_outputs = {
    s3_bucket_id          = "key_id"
    s3_bucket_arn         = "arn::"
    s3_bucket_domain_name = "dsf"
  }
}

dependency "s3-dpxn-bidm-payment-report" {
  config_path                             = "${get_terragrunt_dir()}/../s3-dpxn-bidm-payment-report"
  mock_outputs_allowed_terraform_commands = ["validate", "plan"]
  mock_outputs = {
    s3_bucket_id          = "key_id"
    s3_bucket_arn         = "arn::"
    s3_bucket_domain_name = "dsf"
  }
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  oidc_provider      = true
  create_role        = true
  create_policy      = true
  role_name          = "${local.infra_environment}-${local.app_environment}-payment-reporting-api-role"
  role_description   = "Role used by DLX Payment Reporting API Service (${local.app_environment})"
  policy_name        = "${local.infra_environment}-${local.app_environment}-payment-reporting-api-policy"
  policy_description = "Policy used by DLX Payment Reporting API Service IAM Role (${local.app_environment})"
  role_path          = "/"
  policy_statement = templatefile("policy/statement.tftpl", {
    kms-dpxn-payment-reporting-key-arn                            = dependency.kms-dpxn-payment-reporting.outputs.kms_arn,
    s3-dpxn-userreportdata-bucket-arn                             = dependency.s3-dpxn-userreportdata.outputs.s3_bucket_arn,
    s3-dpxn-bidm-payment-report-bucket-arn                        = dependency.s3-dpxn-bidm-payment-report.outputs.s3_bucket_arn,
    logstream-dpxn-payment-reporting-api-cloudwatch-log-group-arn = dependency.logstream-dpxn-payment-reporting-api-cloudwatch-log-group.outputs.cloudwatch_log_group_arn
  })
  oidc_fully_qualified_subjects = [
    "system:serviceaccount:external:dlx-payment-reporting-api"
  ]
  oidc_fully_qualified_audiences = [
    "sts.amazonaws.com"
  ]
  provider_url = dependency.eks-fargate-cluster.outputs.oidc_role_url

  tags = {
    "TerraformPath" = path_relative_to_include()
  }
}
