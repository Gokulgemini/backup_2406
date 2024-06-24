locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))

  # Extract out common variables for reuse
  env = local.environment_vars.locals.chef_environment
  app_env = local.environment_vars.locals.app_environment
  modules_version = "1.0.0"
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "git::git@bitbucket.org:deluxe-development/aws-sftp-users.git//?ref=${local.modules_version}"
}

# Include all settings from the root terragrunt.hcl file
include {
  path = find_in_parent_folders()
}

dependency "s3-dpxn-acknowledgement" {
  config_path = "${get_terragrunt_dir()}/../s3-dpxn-acknowledgement"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    s3_bucket_id = "key_id"
    s3_bucket_arn = "arn::"
    s3_bucket_domain_name = "dsf"
  }
}

dependency "transfer-sftp-dpxn-acknowledgement" {
  config_path = "${get_terragrunt_dir()}/../transfer-sftp-dpxn-acknowledgement"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    sftp_id = "s-01234567890abcdef"
    sftp_transfer_server_iam_role = "arn:aws:transfer:us-east-1:123456789012:server/s-01234567890abcdef"
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
  users = [{
      s3_bucket_id = dependency.s3-dpxn-acknowledgement.outputs.s3_bucket_id
      public_key = "<public_key>"
      user_name = "<user_name>"
      sftp_id = dependency.transfer-sftp-dpxn-acknowledgement.outputs.sftp_id
      transfer_server_role_arn = dependency.transfer-sftp-dpxn-acknowledgement.outputs.sftp_transfer_server_iam_role
      tags = merge(dependency.common-tags.outputs.common_tags, {
        "TerraformPath" = path_relative_to_include()
      })
    }
  ]
}
