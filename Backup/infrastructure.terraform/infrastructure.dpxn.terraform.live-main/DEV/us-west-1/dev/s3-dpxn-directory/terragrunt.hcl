locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))

  # Extract out common variables for reuse
  infra_environment = lower(replace(local.environment_vars.locals.infra_environment, "_", "-"))
  app_environment   = lower(local.environment_vars.locals.app_environment)
  bucket_name       = "${local.infra_environment}-${local.app_environment}-directory"
  modules_version   = "2.0.1"
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "git::git@bitbucket.org:deluxe-development/aws-s3-bucket.git//?ref=${local.modules_version}"
}

# Include all settings from the root terragrunt.hcl file
include {
  path = find_in_parent_folders()
}

dependency "common-tags" {
  config_path                             = "${get_terragrunt_dir()}/../overlay-common-tags"
  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy", "plan-all"]
  mock_outputs = {
    asg_common_tags = [
      {
        key                 = "AccountId"
        propagate_at_launch = true
        value               = "3432"
      }
    ]
    common_tags = {
      "AppName" = "DPXN"
    }
  }
}

dependency "kms-dpxn-directory" {
  config_path                             = "${get_terragrunt_dir()}/../kms-dpxn-directory"
  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy"]
  mock_outputs = {
    kms_arn           = "arn::"
    kms_id            = "key_id"
    kms_key_alias_arn = "arn::"
  }
}

dependency "sqs-dpxn-directory" {
  config_path                             = "${get_terragrunt_dir()}/../sqs-dpxn-directory"
  mock_outputs_allowed_terraform_commands = ["validate", "plan", "plan-all"]
  mock_outputs = {
    queue_arn            = "arn::"
    queue_id             = "12344"
    queue                = "queue"
    deadletter_queue_arn = "arn::"
    deadletter_queue_id  = "12344"
    deadletter_queue     = "d_queue"
  }
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  bucket_name        = local.bucket_name
  acl                = "private"
  versioning_enabled = true
  encryption_policy  = true
  sqs_notifications = {
    sqs-dpxn-directory = {
      events    = ["s3:ObjectCreated:*"]
      queue_arn = dependency.sqs-dpxn-directory.outputs.queue_arn
    }
  }
  kms_master_key_id = dependency.kms-dpxn-directory.outputs.kms_arn
  tags = merge(dependency.common-tags.outputs.common_tags, {
    "TerraformPath" = path_relative_to_include()
    "Name"          = local.bucket_name
  })
}
