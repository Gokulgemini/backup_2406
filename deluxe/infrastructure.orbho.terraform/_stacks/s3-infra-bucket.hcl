locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))
  account_vars     = read_terragrunt_config(find_in_parent_folders("account.hcl"))
  region_vars      = read_terragrunt_config(find_in_parent_folders("region.hcl"))

  # Extract out common variables for reuse
  app_name        = lower(local.account_vars.locals.app_name)
  account_name    = lower(local.account_vars.locals.account_name)
  aws_region      = lower(local.region_vars.locals.aws_region)
  bucket_name     = "${local.app_name}-infra-${local.aws_region}-${local.account_name}"
  source_base_url = "git::ssh://git@bitbucket.org/deluxe-development/aws-s3-bucket.git//"
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

dependency "common-tags" {
  config_path = "${get_terragrunt_dir()}/../../_global/common-tags"

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
  bucket_name        = local.bucket_name
  acl                = null 
  create_object      = true
  key                = "backup/"
  lifecycle_rule = [
    {
      id      = "expire_old_backups"
      enabled = true
      prefix  = "backup/"
      tags = {
        Expiration = "true"
      }
      
      expiration = {
        days = 30
      }     
    }
  ]
  encryption_policy  = true
  kms_master_key_id  = dependency.kms-non-db-key.outputs.kms_arn
  additional_statements = [
    {
      sid = "AllowIAMRoleUpload"
      actions = [
        "s3:PutObject",
        "s3:PutObjectAcl",
        "s3:PutObjectTagging"
      ]
      effect = "Allow"
      principals = [{
        type = "AWS"
        identifiers = [
          "arn:aws:iam::${local.account_vars.locals.aws_account_id}:role/Terraform-Admin-Role"
        ]
      }]
      resources = [
        "arn:aws:s3:::${local.bucket_name}/*"
      ]
      condition = []
    }
  ]
  tags = merge(dependency.common-tags.outputs.common_tags, {
    "TerraformPath" = path_relative_to_include()
    "Name"          = local.bucket_name
  })
}
