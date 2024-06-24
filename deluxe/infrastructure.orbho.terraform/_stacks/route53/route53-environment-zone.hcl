locals {
  # Automatically load environment-level variables  
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))
  app_environment  = lower(local.environment_vars.locals.app_environment)
  source_base_url  = "git::ssh://git@bitbucket.org/deluxe-development/aws-route53-zone.git//"
}

dependency "common-tags" {
  config_path = "${get_terragrunt_dir()}/../overlay-common-tags"

  mock_outputs_allowed_terraform_commands = ["validate", "destroy"]
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

dependency "route53-account-zone" {
  config_path = "${get_terragrunt_dir()}/../../_global/route53-account-zone"

  mock_outputs_allowed_terraform_commands = ["validate", "destroy", "plan-all"]
  mock_outputs = {
    dns_name = "test"
    zone_ids = {
      "internal" = "123"
      "private"  = "123"
    }
  }
}

dependency "vpc-info" {
  config_path = "${get_terragrunt_dir()}/../../_global/vpc-info"

  mock_outputs_allowed_terraform_commands = ["validate", "destroy", "plan-all"]
  mock_outputs = {
    vpc_id = "123"
  }
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  dns_name        = "dep.${dependency.route53-account-zone.outputs.dns_name}"
  tags            = dependency.common-tags.outputs.common_tags
  parent_zone_ids = dependency.route53-account-zone.outputs.zone_ids
  vpc_id          = dependency.vpc-info.outputs.vpc_id
  create_private  = true
}

