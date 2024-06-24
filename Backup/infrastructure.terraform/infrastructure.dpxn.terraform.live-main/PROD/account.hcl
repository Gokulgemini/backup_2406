# Set account-wide variables. These are automatically pulled in to configure the remote state bucket in the root
# terragrunt.hcl configuration.

locals {
  account_name = "PROD"
  aws_account_id = "323265901760"
  aws_profile = "dpxn_lz_prod"
  # Remote State (rs) Bucket Name
  rs_bucket_name = "deluxe-vdms-tfstate"
  vpc_remote_state_key = "GS-DPXN-PROD"
  app_name = "DPXN"
  account_tags = {
    "AccountId" = local.aws_account_id
    "Environment" = "Production"
    "AppName" = local.app_name
    "OSType" = "Linux"
    "OSFlavor" = "Ubuntu 22.04"
    "OSVariant" = "Ubuntu 22.04"
    "Domain" = "none"
    "DeploymentTeam" = "DevOps_DPX"
    "Application" = "DPXN"
    "PamReady" = "True"
  }
  pci_account_number = "916109733785"
  pci_app_subnet_cidrs = [ "10.184.157.0/24", "10.184.156.0/24" ]
}
