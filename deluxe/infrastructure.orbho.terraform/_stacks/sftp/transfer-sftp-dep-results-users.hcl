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

dependency "s3-dep-results" {
  config_path = "${get_terragrunt_dir()}/../s3-dep-results"

  mock_outputs_allowed_terraform_commands = ["validate", "plan"]
  mock_outputs = {
    s3_bucket_id          = "key_id"
    s3_bucket_arn         = "arn::"
    s3_bucket_domain_name = "dsf"
  }
}

dependency "transfer-sftp-dep-results" {
  config_path = "${get_terragrunt_dir()}/../transfer-sftp-dep-results"

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
    s3_bucket_id             = dependency.s3-dep-results.outputs.s3_bucket_id
    public_key               = "ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAACAQC7ffLkqMlDBifncUm7KnisNi4BDpsntNlh5SRe4zRQJzDRkPRmN49XRRWWWUTQpyJbTZlvEm2Tsl+fbmAXEA5mTGIT0erMRlfZzBlENVuU4XwQmiqokR/VXrL829D18gQdObufojFPozsiP/XMD9DKv4zcW8IXAUhvUKEsam/rX40LawFsBfWhyhq2ieYiVZbBHlE6gS8EmdGEXQzWtrEAA3NoHW2c3/KsnX6P2t9d1RH+zm8HdP9badB0l8yrdZ+eQgTU7SnRVF5fGG08j1S2xx49hg/KZMwCeLHQFBoJYwmErFUiI1sUohQXrUaTxd3OZ03TTX2yVPikcSD9czkxD8o/JLpE6ncys2/8dOoIbuFqvm9Dvg2rhfhUhExYU4mUepu1FxfM9J2ip6CO1GRPBts+brFTXic+GkAhPIYZlWpE6DFSUpQpb/sLBcn4hrzu3AcPjPJ3glu2wuuG9KVuxvMJZx9z6fIAXTbOnXtJtihN98l6+M7IeNlQtxYGnvX7l/37qUVF0hFMFhX/gmqc4BqACh0NT5LEYkQKLg/xyZ++3Xu3O18TAE8JnqV9mRR/QM2if3tYbOpy5eMhP7uHw5Xrl8eQUOIXTfGsuG2qTOhxjulyOIARsw3u81IJlEpzV0hKzHIELFG66XKw7Eo1I2FpdMxW5HLhHmUOJdbn2w== t475243@TXD76118"
    user_name                = "rps_results"
    sftp_id                  = dependency.transfer-sftp-dep-results.outputs.sftp_id
    transfer_server_role_arn = dependency.transfer-sftp-dep-results.outputs.sftp_transfer_server_iam_role
    tags = merge(dependency.common-tags.outputs.common_tags, {
      "TerraformPath" = path_relative_to_include()
    })
  }]
}

