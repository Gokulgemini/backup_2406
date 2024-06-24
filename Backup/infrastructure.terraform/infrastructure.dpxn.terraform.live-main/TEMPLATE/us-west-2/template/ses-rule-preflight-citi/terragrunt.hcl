locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))

  # Extract out common variables for reuse
  infra_env = replace(local.environment_vars.locals.chef_environment, "_", "-")
  app_environment = local.environment_vars.locals.app_environment

  modules_version = local.environment_vars.locals.dpxn_modules_version
  customer = "citi"

}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "git::git@github.com:verifyvalid/infrastructure.dpx_fulfillment.terraform.modules.git//ses-rule?ref=${local.modules_version}"
}

# Include all settings from the root terragrunt.hcl file
include {
  path = find_in_parent_folders()
}

dependency "ses-ruleset-preflight" {
  config_path = "${get_terragrunt_dir()}/../ses-ruleset-preflight"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    rule_set_name = "1234567890"
  }
}

dependency "s3-dpxn-email-inbound" {
  config_path = "${get_terragrunt_dir()}/../s3-dpxn-email-inbound"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    s3_bucket_id = "key_id"
    s3_bucket_arn = "arn::"
    s3_bucket_domain_name = "dsf"
  }
}

dependency "lambda-dpxn-email-inbound-handler-citi" {
  config_path = "${get_terragrunt_dir()}/../lambda-dpxn-email-inbound-handler-citi"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    arn = "arn:aws:lambda:us-west-1:123456789123:function:functioname"
    qualified_arn = "arn::"
  }
}

dependency "kms-non-db-key" {
  config_path = "${get_terragrunt_dir()}/../../_global/kms-non-db-key"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    kms_arn = "arn:aws:s3:::mockplan-bucket"
    kms_id = "key_id"
    kms_key_alias_arn = "arn::"
  }
}

dependency "ses-domain" {
  config_path = "${get_terragrunt_dir()}/../ses-domain"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    ses_verification_token = "dfd23sdfs2"
    ses_domain_name = "dpxn.echecks.com"
  }
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  rule_set_name = dependency.ses-ruleset-preflight.outputs.rule_set_name
  rule_name = "dpxn-${local.infra_env}-${local.app_environment}-${local.customer}"
  recipients = ["${local.customer}.${md5(element(concat([local.customer],[dependency.ses-domain.outputs.ses_domain_name]),0))}@${dependency.ses-domain.outputs.ses_domain_name}"]
  lambda_action = [
    {
      function_arn = dependency.lambda-dpxn-email-inbound-handler-citi.outputs.arn
      invocation_type = "RequestResponse"
      topic_arn             = null
      position = 1
    }
  ]

  s3_action = [
    {
        bucket_name           = dependency.s3-dpxn-email-inbound.outputs.s3_bucket_id
        kms_key_arn           = dependency.kms-non-db-key.outputs.kms_arn
        object_key_prefix     = local.customer
        topic_arn             = null
        position              = 2
    }
  ]
}
