# Set common variables for the environment. This is automatically pulled in in the root terragrunt.hcl configuration to
# feed forward to the child modules.
locals {
  app_environment = "global"
  infra_environment = "dev"
  env_tags = {
  }
  vpc_remote_state_key = "637423585162-aft-global-customizations"
}
