# Set common variables for the environment. This is automatically pulled in in the root terragrunt.hcl configuration to
# feed forward to the child modules.
locals {
  # Chef configs
  chef_org            = "dpxn-prod"
  chef_server_url     = "chef-west.gs-echecks.com"
  chef_validator_name = "dpxn-prod-validator.pem"
  chef_environment    = "dpxn_xuat"
  infra_environment   = "dpxn_xuat"
  app_environment     = "xuat"
  env_tags = {
    "InfraEnvironment" = local.infra_environment
    "AppEnvironment"   = local.app_environment
    "ChefEnvironment"  = local.chef_environment
  }
  modules_version      = "main"
  dpxn_modules_version = "main"
  ssl_cert_api_portal_ui_resque_ui = "arn:aws:acm:us-west-1:323265901760:certificate/c3c11772-6784-40d1-8779-4793dd778754"
}
