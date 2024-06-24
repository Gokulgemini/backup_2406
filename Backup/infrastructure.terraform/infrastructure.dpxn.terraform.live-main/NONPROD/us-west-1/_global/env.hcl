# Set common variables for the environment. This is automatically pulled in in the root terragrunt.hcl configuration to
# feed forward to the child modules.
locals {
  chef_environment = "default"
  app_environment = "global"
  env_tags = {
    "InfraEnvironment" = local.chef_environment
    "AppEnvironment" = local.app_environment
  }
  modules_version = "main"
}
