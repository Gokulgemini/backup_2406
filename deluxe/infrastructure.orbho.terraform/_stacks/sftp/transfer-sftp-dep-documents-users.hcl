locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))
  account_vars     = read_terragrunt_config(find_in_parent_folders("account.hcl"))
  region_vars      = read_terragrunt_config(find_in_parent_folders("region.hcl"))

  # Extract out common variables for reuse
  app_environment   = lower(local.environment_vars.locals.app_environment)
  infra_environment = lower(local.account_vars.locals.account_name)
  source_base_url   = "git::ssh://git@bitbucket.org/deluxe-development/aws-sftp-users.git//"
}

dependency "s3-dep-documents" {
  config_path = "${get_terragrunt_dir()}/../s3-dep-documents"

  mock_outputs_allowed_terraform_commands = ["validate", "plan"]
  mock_outputs = {
    s3_bucket_id          = "key_id"
    s3_bucket_arn         = "arn::"
    s3_bucket_domain_name = "dsf"
  }
}

dependency "transfer-sftp-dep-documents" {
  config_path = "${get_terragrunt_dir()}/../transfer-sftp-dep-documents"

  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy"]
  mock_outputs = {
    sftp_id                       = "s-01234567890abcdef"
    sftp_transfer_server_iam_role = "arn:aws:transfer:us-east-1:123456789012:server/s-01234567890abcdef"
  }
}

dependency "common-tags" {
  config_path = "${get_terragrunt_dir()}/../overlay-common-tags"

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
      "AppName" = "DEP"
    }
  }
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  users = [{
    s3_bucket_id             = dependency.s3-dep-documents.outputs.s3_bucket_id
    public_key               = "ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAACAQDRmZw3uJ012RnUVakDL2ZoNWYEjToyHjsWmd+pUXmBGWlkvHNCA4n2lVU5C68gidu1peEPX5i5EfuJUi0YBApmNizfo/1NOiAWAN+yFKWukO1Owj8Wep4UtOd1rBbkjA3hPDGf4wNNo2Ez4RwN27LfhWK7RaziMQw90I1w3BkIENK/HG5acNmHnCig/ZXytYBfhgnHJqzamxWw8+oIwiUtPMBA6RDgULt+j4StKg6/SSmLEQaMZYqH1AX+62qRHWTXRTy4MrbPY2cOV7K5AkYED7hZGqfb2BrwmmiBLouGIiLV4lYSLuaMI1XeAAmXFKDWOunBvqiC73e0gb25rq514qmQlxKrtXSt7qf7mZRdWFY1yNTSNFtitcKiqjun3DokHCXpiffId5MsqRARwEjMA4BSE0F3VMvOsYkvGH8xpgS/PRJIRaQW5XgAgW7CwPuUNiXj2in1/CS1I+VXDMm6LpzveuwEAwXkxPC89t9wcfucJ8ufZ7BhLoQpsaSMgGWYz9uMub97az3pUfcHYbbJVI0l+7Cpn5v9HmOveS8WV+HFHwrD7u+kz42oj6cs4L2ncLBss5R3GQ/KI2BXbZBBPXpr/n160RXY6Gufr/L1iaCtM6iEyqL3r5sdFMvJSb1SJcw/b+UPLj5J0VnKFqKiFJl/WIz+nwZ+JXOph0YIjw== t475243@TXD76118"
    user_name                = "rps_documents"
    sftp_id                  = dependency.transfer-sftp-dep-documents.outputs.sftp_id
    transfer_server_role_arn = dependency.transfer-sftp-dep-documents.outputs.sftp_transfer_server_iam_role
    tags = merge(dependency.common-tags.outputs.common_tags, {
      "TerraformPath" = path_relative_to_include()
    })
  }]
}

