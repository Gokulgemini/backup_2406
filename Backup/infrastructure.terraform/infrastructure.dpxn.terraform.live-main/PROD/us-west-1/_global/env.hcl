# Set common variables for the environment. This is automatically pulled in in the root terragrunt.hcl configuration to
# feed forward to the child modules.
locals {
  chef_environment = "dpxn_production"
  app_environment = "production"
  env_tags = {
    "InfraEnvironment" = local.chef_environment
    "AppEnvironment" = local.app_environment
  }
  modules_version = "1.1.2"
  dpxn_modules_version = "1.0.0"
}
