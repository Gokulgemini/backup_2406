locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))

  # Extract out common variables for reuse
  parsed_env = replace(local.environment_vars.locals.chef_environment, "_", "-")
  chef_environment = local.environment_vars.locals.chef_environment
  app_environment = local.environment_vars.locals.app_environment
  modules_version = "1.0.1"
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "git::ssh://git@bitbucket.deluxe.com:7999/dtf/aws-route53.git//?ref=${local.modules_version}"

  extra_arguments "common_var" {
    commands = [
      "apply",
      "apply-all",
      "plan-all",
      "destroy-all",
      "plan",
      "import",
      "push",
      "refresh"
    ]
  }
}

# Include all settings from the root terragrunt.hcl file
include {
  path = find_in_parent_folders()
}

dependency "common-tags" {
  config_path = "${get_terragrunt_dir()}/../overlay-common-tags"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    common_tags = {
      "AppName" = "DPXN"
    }
  }
}

dependency "route53-environment-zone" {
  config_path = "${get_terragrunt_dir()}/../route53-environment-zone"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    zone_ids = "123"
    zone_arn = "abc"
    dns_name = "www"
    nameservers = "xyz"
  }
}

dependency "transfer-sftp-dpxn-acknowledgement" {
  config_path = "${get_terragrunt_dir()}/../transfer-sftp-dpxn-acknowledgement"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    sftp_arn = "abc"
    sftp_endpoint = "abc"
    sftp_host_key_fingerprint = "abc"
    sftp_id = "123"
    sftp_transfer_server_iam_role = "abc"
    vpc_endpoint_dns = "vpce-abc"
    vpc_endpoint_id = "vpce-123"
  }
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  create_zones = false
  create_public_record = true
  create_private_record = true
  zone_ids = dependency.route53-environment-zone.outputs.zone_ids
  records_jsonencoded = jsonencode([
    {
      name           = "${local.parsed_env}-${local.app_environment}-sftp-ack"
      type           = "CNAME"
      ttl            = 60
      records        = [dependency.transfer-sftp-dpxn-acknowledgement.outputs.vpc_endpoint_dns]
    }
  ])
  tags = merge({ "TerraformPath" = path_relative_to_include() }, dependency.common-tags.outputs.common_tags)
}
