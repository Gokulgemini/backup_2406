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
}
