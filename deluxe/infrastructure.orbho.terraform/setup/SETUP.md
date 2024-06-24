# Terraform State Storage Setup

In order to be able to use S3 backend for terraform we need to setup an S3 bucket to use.  In addition we use a dynamodb table to support locking.  Details on what is needed is in the [terraform documentation.](https://www.terraform.io/docs/language/settings/backends/s3.html)

The script by the name setup/create_terraform_state_storage will configure the S3 bucket passed in and creates a dynamodb table to be used. In addition you have to have a policy statement file with the same name as your profile you are passing in.  This policy file will give permissions to the bucket to each relevant Terraform-Admin-role for the accounts that will be using this bucket.

```bash
# ./setup/create_terraform_state_storage dpxn-infra-dev dpxn_nonpci_lz_dev us-west-1

./setup/create_terraform_state_storage <bucket-name> <aws_credential_profile> <aws_region>
```
You only need to run this once in while logged into the AWS account you want to store all of your state information for your accounts.