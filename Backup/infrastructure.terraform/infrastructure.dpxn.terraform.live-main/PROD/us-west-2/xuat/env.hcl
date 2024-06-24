# Set common variables for the environment. This is automatically pulled in in the root terragrunt.hcl configuration to
# feed forward to the child modules.
locals {
  # Chef configs
  chef_org = "dpxn-prod"
  chef_server_url = "chef-west.gs-echecks.com"
  chef_validator_name = "dpxn-prod-validator.pem"
  chef_environment = "dpxn_xuat"
  app_environment = "xuat"
  env_tags = {
    "InfraEnvironment" = local.chef_environment
    "AppEnvironment" = local.app_environment
  }
  modules_version = "v21.2.1"
  dpxn_modules_version = "v21.2.8"
}
