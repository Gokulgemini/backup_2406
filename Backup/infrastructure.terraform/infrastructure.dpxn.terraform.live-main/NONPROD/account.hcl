# Set account-wide variables. These are automatically pulled in to configure the remote state bucket in the root
# terragrunt.hcl configuration.

locals {
  account_name = "NONPROD"
  aws_account_id = "928017146542"
  aws_profile = "dpxn_lz_nonprod"
  app_name = "DPXN"
  account_tags = {
    "AccountId" = local.aws_account_id
    "Environment" = "Test"
    "AppName" = local.app_name
    "OSType" = "Linux"
    "OSFlavor" = "Ubuntu 22.04"
    "OSVariant" = "Ubuntu 22.04"
    "Domain" = "none"
    "DeploymentTeam" = "DevOps_DPX"
    "Application" = "DPXN"
    "PamReady" = "True"
  }
}
