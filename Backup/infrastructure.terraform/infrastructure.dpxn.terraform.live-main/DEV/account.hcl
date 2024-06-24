# Set account-wide variables. These are automatically pulled in to configure the remote state bucket in the root
# terragrunt.hcl configuration.

locals {
  account_name = "DEV"
  aws_account_id = "112151777763"
  aws_profile = "dpxn_lz_dev"
  # Remote State (rs) Bucket Name
  rs_bucket_name = "deluxe-vdms-tfstate"
  vpc_remote_state_key = "GS-DPXN-DEV"
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
  pci_account_number = "991103392360"
  pci_app_subnet_cidrs = [ "10.184.148.0/24", "10.184.149.0/24" ]
}
