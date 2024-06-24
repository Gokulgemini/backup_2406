# Set account-wide variables. These are automatically pulled in to configure the remote state bucket in the root
# terragrunt.hcl configuration.
 
locals {
  account_name   = "DEV"
  aws_account_id = "637423585162"
  app_name       = "DEP"
  certificate_arn = "arn:aws:acm:us-east-1:637423585162:certificate/40bafb75-a82d-46ca-a7b2-4f524a28d785"
  
  # Remote State (rs) Bucket Name
  rs_bucket_name       = "aft-backend-560875144966-primary-region"
  vpc_remote_state_key = "637423585162-aft-global-customizations"
  vpc_id               = "vpc-092680ef1316e4316"
 
  account_tags = {
    "AccountId"      = local.aws_account_id
    "Environment"    = "Development"
    "AppName"        = local.app_name
    "Application"    = "devops-orbo"
  }
}
