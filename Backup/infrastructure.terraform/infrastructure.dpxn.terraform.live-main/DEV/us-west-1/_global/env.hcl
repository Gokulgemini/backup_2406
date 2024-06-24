# Set common variables for the environment. This is automatically pulled in in the root terragrunt.hcl configuration to
# feed forward to the child modules.
locals {
  chef_environment = "dpxn_preproduction"
  app_environment = "preproduction"
  env_tags = {
    "InfraEnvironment" = local.chef_environment
    "AppEnvironment" = local.app_environment
  }
  modules_version = "main"
}
