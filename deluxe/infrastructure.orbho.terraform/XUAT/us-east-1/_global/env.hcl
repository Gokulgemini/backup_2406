# Set common variables for the environment. This is automatically pulled in in the root terragrunt.hcl configuration to
# feed forward to the child modules.
locals {
  app_environment = "global"
  infra_environment = "xuat"
  env_tags = {
  }
  vpc_remote_state_key = "058264534673-aft-global-customizations"
}
