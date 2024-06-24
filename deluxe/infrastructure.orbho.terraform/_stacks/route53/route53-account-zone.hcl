locals {
  # Automatically load environment-level variables
  account_vars     = read_terragrunt_config(find_in_parent_folders("account.hcl"))
  region_vars      = read_terragrunt_config(find_in_parent_folders("region.hcl"))
 
  # Extract out common variables for reuse
  app_name          = lower(local.account_vars.locals.app_name)
  account_name      = lower(local.account_vars.locals.account_name)
  dns_top_domain    = "${local.app_name}.${local.account_name}"
  
  source_base_url   = "git::ssh://git@bitbucket.org/deluxe-development/aws-route53-zone.git//"
}

dependency "common-tags" {
  config_path = "${get_terragrunt_dir()}/../common-tags"

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

dependency "vpc-info" {
  config_path = "${get_terragrunt_dir()}/../vpc-info"

  mock_outputs_allowed_terraform_commands = ["validate", "destroy", "plan-all"]
  mock_outputs = {
    vpc_id = "123"
  }
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  dns_name       = "${local.dns_top_domain}.tm.deluxe.com"
  tags           = dependency.common-tags.outputs.common_tags
  vpc_id         = dependency.vpc-info.outputs.vpc_id
  create_private = true
}

