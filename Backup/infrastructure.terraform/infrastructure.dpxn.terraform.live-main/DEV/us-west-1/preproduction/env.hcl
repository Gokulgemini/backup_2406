# Set common variables for the environment. This is automatically pulled in in the root terragrunt.hcl configuration to
# feed forward to the child modules.
locals {
  # Chef configs
  chef_org = "dpxn-dev"
  chef_server_url = "chef-west.gs-echecks.com"
  chef_validator_name = "dpxn-dev-validator.pem"
  chef_environment = "dpxn_preproduction"
  app_environment = "preproduction"
  env_tags = {
    "InfraEnvironment" = local.chef_environment
    "AppEnvironment" = local.app_environment
  }
  modules_version = "main"
  dpxn_modules_version = "main"
  ssl_cert_api_portal_ui_resque_ui = "arn:aws:acm:us-west-1:112151777763:certificate/8239abb2-2c1c-4099-83ae-daa8104977f2"
}
