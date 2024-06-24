# Set account-wide variables. These are automatically pulled in to configure the remote state bucket in the root
# terragrunt.hcl configuration.

locals {
  account_name = "<ENV>"
  aws_account_id = "<AccountId>"
  aws_profile = "<local_aws_profile>"
  # Remote State (rs) Bucket Name
  rs_bucket_name = "<remote_state_bucket_name>"
  vpc_remote_state_key = "<vpc_remote_state_key>"
  app_name = "DPXN"
  account_tags = {
    "AccountId" = local.aws_account_id
    "Environment" = "<ENV>"
    "AppName" = local.app_name
    "OSType" = "Linux"
    "OSFlavor" = "Ubuntu 22.04"
    "OSVariant" = "Ubuntu 22.04"
    "Domain" = "none"
    "DeploymentTeam" = "DevOps_DPX"
    "Application" = "DPXN"
    "PamReady" = "True"
  }
  pci_account_number = "<pci_account_number>"
  pci_app_subnet_cidrs = [ "app-subnet-1", "app-subnet-2" ]
}
