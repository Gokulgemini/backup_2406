# Set account-wide variables. These are automatically pulled in to configure the remote state bucket in the root
# terragrunt.hcl configuration.
 
locals {
  account_name   = "XUAT"
  aws_account_id = "058264534673"
  app_name       = "DEP"
  certificate_arn = "arn:aws:acm:us-east-1:058264534673:certificate/b8457196-7c49-4e21-8f6f-839ff07a049e"
 
  # Remote State (rs) Bucket Name
  rs_bucket_name       = "aft-backend-560875144966-primary-region"
  vpc_remote_state_key = "058264534673-aft-global-customizations"
  vpc_id               = "vpc-041a1dd0a0a72db5f"
 
  account_tags = {
    "AccountId"      = local.aws_account_id
    "Environment"    = "XUAT"
    "AppName"        = local.app_name
    "Application"    = "devops-orbo"
    "OSType"         = "Windows"
    "OSFlavor"       = "Windows Server 2022"
    "Domain"         = "deluxe.com"
  }
}
