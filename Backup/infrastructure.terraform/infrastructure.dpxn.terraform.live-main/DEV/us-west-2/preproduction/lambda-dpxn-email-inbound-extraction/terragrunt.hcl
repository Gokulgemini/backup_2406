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
  source = "git::git@github.com:verifyvalid/infrastructure.dpx_fulfillment.terraform.modules.git//lambda-s3-event-email-attachment-extraction?ref=${local.modules_version}"
}

# Include all settings from the root terragrunt.hcl file
include {
  path = find_in_parent_folders()
}


dependency "s3-dpxn-email-preflight" {
  config_path = "${get_terragrunt_dir()}/../../../us-west-1/${local.app_env}/s3-dpxn-email-preflight"

  mock_outputs_allowed_terraform_commands = ["validate","plan"]
  mock_outputs = {
    s3_bucket_id = "key_id"
    s3_bucket_arn = "arn::"
    s3_bucket_domain_name = "dsf"
  }
}

dependency "s3-dpxn-email-inbound" {
  config_path = "${get_terragrunt_dir()}/../s3-dpxn-email-inbound"

  mock_outputs_allowed_terraform_commands = ["validate","plan"]
  mock_outputs = {
    s3_bucket_id = "key_id"
    s3_bucket_arn = "arn::"
    s3_bucket_domain_name = "dsf"
  }
}

dependency "kms-non-db-key-usw1" {
  config_path = "${get_terragrunt_dir()}/../../../us-west-1/_global/kms-non-db-key"

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
  function_name = "${local.env}-${local.app_env}-email-inbound-extraction"
  filename_regex = "^.{1,500}.((xlsx)|(XLSX))$"
  destination_bucket = dependency.s3-dpxn-email-preflight.outputs.s3_bucket_id
  key_prefix = "ses/citi/recon"
  bucket_name = dependency.s3-dpxn-email-inbound.outputs.s3_bucket_id
  memory_size = 512
  policy_statement = <<EOF
{
  "Version": "2012-10-17",
  "Statement": [
      {
        "Effect": "Allow",
        "Action": [
          "s3:PutObject",
          "s3:PutObjectAcl"
        ],
        "Resource": "${dependency.s3-dpxn-email-preflight.outputs.s3_bucket_arn}/*"
      },
      {
      "Action": [
        "kms:Encrypt",
        "kms:Decrypt"
      ],
      "Effect": "Allow",
      "Resource": "${dependency.kms-non-db-key-usw1.outputs.kms_arn}"
      },
      {
      "Action": [
        "s3:GetObject",
        "s3:GetObjectAcl"
      ],
      "Effect": "Allow",
      "Resource": "${dependency.s3-dpxn-email-inbound.outputs.s3_bucket_arn}/*"
      }
  ]
}
EOF
  tags = merge(dependency.common-tags.outputs.common_tags, { "TerraformPath" = path_relative_to_include() } )
}
