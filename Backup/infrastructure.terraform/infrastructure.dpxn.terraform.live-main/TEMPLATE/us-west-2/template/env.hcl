# Set common variables for the environment. This is automatically pulled in in the root terragrunt.hcl configuration to
# feed forward to the child modules.
locals {
  # Chef configs
  chef_org = "<chef_org>"
  chef_server_url = "<chef_server_url>"
  chef_validator_name = "<chef_validator_name>"
  chef_environment = "<chef_env>"
  app_environment = "<app_environment>"
  env_tags = {
    "InfraEnvironment" = local.chef_environment
    "AppEnvironment" = local.app_environment
  }
  modules_version = "<modules_version>"
  dpxn_modules_version = "<modules_version>"
}
