# Set account-wide variables. These are automatically pulled in to configure the remote state bucket in the root
# terragrunt.hcl configuration.
 
locals {
  account_name   = "PROD"
  aws_account_id = "851725215854"
  app_name       = "DEP"
 
  # Remote State (rs) Bucket Name
  rs_bucket_name       = "aft-backend-560875144966-primary-region"
  vpc_remote_state_key = "851725215854-aft-global-customizations"
 
  account_tags = {
    "AccountId"      = local.aws_account_id
    "Environment"    = "Production"
    "AppName"        = local.app_name
    "Application"    = "devops-orbo"
    "OSType"         = "Windows"
    "OSFlavor"       = "Windows Server 2022"
    "Domain"         = "deluxe.com"
  }
}
