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
  source = "git::git@bitbucket.org:deluxe-development/aws-route53.git//?ref=${local.modules_version}"

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
    zone_ids = {
      "internal" = "123"
      "private"  = "123"
    }
    zone_arn = "abc"
    dns_name = "www"
    nameservers = "xyz"
  }
}

dependency "ec2-dpxn-api" {
  config_path = "${get_terragrunt_dir()}/../ec2-dpxn-api"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    alb_arn      = "abc"
    alb_dns_name = "abc"
    alb_id       = "abc"
    asg_arn      = "abc"
    asg_id       = "abc"
  }
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  create_zone = false
  create_public_record = true
  create_private_record = true
  zone_ids = dependency.route53-environment-zone.outputs.zone_ids
  records_jsonencoded = jsonencode([
    {
      name           = "dpxn-api"
      type           = "CNAME"
      ttl            = 60
      records        = [dependency.ec2-dpxn-api.outputs.alb_dns_name]
    }
  ])
  tags = merge({ "TerraformPath" = path_relative_to_include() }, dependency.common-tags.outputs.common_tags)
}
