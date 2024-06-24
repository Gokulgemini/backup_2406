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

dependency "alb-dep-services" {
  config_path = "${get_terragrunt_dir()}/../alb-dep-services"

  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy"]
  mock_outputs = {
    loadbalancer_dns_name = "abc"
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
      name    = "orbo-recognition-service"
      type    = "CNAME"
      ttl     = 60
      records = [dependency.alb-dep-services.outputs.loadbalancer_dns_name]
    },
    {
      name    = "orbo-batch-service"
      type    = "CNAME"
      ttl     = 60
      records = [dependency.alb-dep-services.outputs.loadbalancer_dns_name]
    },
    {
      name    = "alb-services"
      type    = "CNAME"
      ttl     = 60
      records = [dependency.alb-dep-services.outputs.loadbalancer_dns_name]
    },
    {
      name    = "processors"
      type    = "CNAME"
      ttl     = 60
      records = [dependency.alb-dep-services.outputs.loadbalancer_dns_name]
    },
    {
      name    = "orbo-api"
      type    = "CNAME"
      ttl     = 60
      records = [dependency.alb-dep-services.outputs.loadbalancer_dns_name]
    },
    {
      name    = "orbo-ai-primaryengine"
      type    = "CNAME"
      ttl     = 60
      records = [dependency.alb-dep-services.outputs.loadbalancer_dns_name]
    },
    {
      name    = "orbo-ai-secondaryengine"
      type    = "CNAME"
      ttl     = 60
      records = [dependency.alb-dep-services.outputs.loadbalancer_dns_name]
    }
  ])
  tags = merge({ "TerraformPath" = path_relative_to_include() }, dependency.common-tags.outputs.common_tags)
}

