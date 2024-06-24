locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))
  account_vars     = read_terragrunt_config(find_in_parent_folders("account.hcl"))
  region_vars      = read_terragrunt_config(find_in_parent_folders("region.hcl"))

  # Extract out common variables for reuse
  app_environment   = lower(local.environment_vars.locals.app_environment)
  infra_environment = lower(local.account_vars.locals.account_name)
  source_base_url   = "git::ssh://git@bitbucket.org/deluxe-development/aws-kms-key.git//"
}

dependency "common-tags" {
  config_path = "${get_terragrunt_dir()}/../overlay-common-tags"

  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy"]
  mock_outputs = {
    asg_common_tags = [
      {
        key                 = "AccountId"
        propagate_at_launch = true
        value               = "3432"
      }
    ]
    common_tags = {
      "AppName" = "${local.account_vars.locals.app_name}"
    }
  }
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  is_enabled          = true
  enable_key_rotation = true
  kms_alias           = "${local.infra_environment}-dep-orbo-services"
  tags = merge(dependency.common-tags.outputs.common_tags, {
    "TerraformPath" = path_relative_to_include()
  })
  additional_policy = [{
    sid = "AllowS3AccessToKms"
    actions = [
      "kms:Decrypt",
      "kms:GenerateDataKey"
    ]
    effect = "Allow"
    principals = [{
      type = "Service"
      identifiers = [
        "s3.amazonaws.com"
      ]
    }]
    resources = [
      "*"
    ]
    condition = []
    },
    {
      sid = "AllowSQSAccessToKms"
      actions = [
        "kms:Decrypt",
        "kms:GenerateDataKey"
      ]
      effect = "Allow"
      principals = [{
        type = "Service"
        identifiers = [
          "sqs.amazonaws.com"
        ]
      }]
      resources = [
        "*"
      ]
      condition = []
  }]
}

