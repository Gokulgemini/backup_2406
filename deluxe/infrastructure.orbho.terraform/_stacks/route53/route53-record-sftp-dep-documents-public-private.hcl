locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))
  account_vars     = read_terragrunt_config(find_in_parent_folders("account.hcl"))
  region_vars      = read_terragrunt_config(find_in_parent_folders("region.hcl"))

  # Extract out common variables for reuse
  app_environment   = lower(local.environment_vars.locals.app_environment)
  infra_environment = lower(local.account_vars.locals.account_name)
  source_base_url   = "git::ssh://git@bitbucket.org/deluxe-development/aws-route53.git//"
}

dependency "common-tags" {
  config_path = "${get_terragrunt_dir()}/../overlay-common-tags"

  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy"]
  mock_outputs = {
    common_tags = {
      "AppName" = "DEP"
    }
  }
}

dependency "route53-account-zone" {
  config_path = "${get_terragrunt_dir()}/../../_global/route53-account-zone"

  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy"]
  mock_outputs = {
    zone_ids    = "123"
    zone_arn    = "abc"
    dns_name    = "www"
    nameservers = "xyz"
  }
}

dependency "transfer-sftp-dep-documents" {
  config_path = "${get_terragrunt_dir()}/../transfer-sftp-dep-documents"

  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy"]
  mock_outputs = {
    sftp_arn                      = "abc"
    sftp_endpoint                 = "abc"
    sftp_host_key_fingerprint     = "abc"
    sftp_id                       = "123"
    sftp_transfer_server_iam_role = "abc"
    vpc_endpoint_dns              = "vpce-abc"
    vpc_endpoint_id               = "vpce-123"
  }
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  create_zones          = false
  create_public_record  = true
  create_private_record = true
  zone_ids              = dependency.route53-account-zone.outputs.zone_ids
  records_jsonencoded = jsonencode([
    {
      name    = "sftp-dep-prod-documents"
      type    = "CNAME"
      ttl     = 60
      records = [dependency.transfer-sftp-dep-documents.outputs.vpc_endpoint_dns]
    }
  ])
  tags = merge({ "TerraformPath" = path_relative_to_include() }, dependency.common-tags.outputs.common_tags)
}

