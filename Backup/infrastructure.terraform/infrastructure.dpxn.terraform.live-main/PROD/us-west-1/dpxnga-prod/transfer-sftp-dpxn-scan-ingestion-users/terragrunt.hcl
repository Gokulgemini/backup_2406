locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))

  # Extract out common variables for reuse
  env = local.environment_vars.locals.chef_environment
  app_env = local.environment_vars.locals.app_environment
  modules_version = "1.0.0"
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "git::git@bitbucket.org:deluxe-development/aws-sftp-users.git//?ref=${local.modules_version}"
}

# Include all settings from the root terragrunt.hcl file
include {
  path = find_in_parent_folders()
}

dependency "s3-dpxn-scan-ingestion" {
  config_path = "${get_terragrunt_dir()}/../s3-dpxn-scan-ingestion"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    s3_bucket_id = "key_id"
    s3_bucket_arn = "arn::"
    s3_bucket_domain_name = "dsf"
  }
}

dependency "transfer-sftp-dpxn-scan-ingestion" {
  config_path = "${get_terragrunt_dir()}/../transfer-sftp-dpxn-scan-ingestion"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    sftp_id = "s-01234567890abcdef"
    sftp_transfer_server_iam_role = "arn:aws:transfer:us-east-1:123456789012:server/s-01234567890abcdef"
  }
}

dependency "common-tags" {
  config_path = "${get_terragrunt_dir()}/../overlay-common-tags"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy", "plan-all"]
  mock_outputs = {
    asg_common_tags = [
      {
        key = "AccountId"
        propagate_at_launch = true
        value = "3432"
      }
    ]
    common_tags = {
      "AppName" = "DPXN"
    }
  }
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  users = [{
      s3_bucket_id = dependency.s3-dpxn-scan-ingestion.outputs.s3_bucket_id
      public_key = "ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAACAQCmerX0mlZ1rXM/sENl/5dlR+20J/OjQhWh9nY7JmF8Lq834V53p8ycX0SJkaknJmXWcVVYRpgzaKEq4T62R71kpUGB//fpzaGzleszk5B6c/ebYXmSWw7Sthza/nwhQh+CSXwevq5o+/Vn26PqtHO8OOCL40iJh9rzN7kRa+zSO4jNbSK4lC55CadO54wYv0gIT1+SVp/K5LtZOfPFV9mqdJY4FR+C0zenZgbShgj6vpqQYnrElbP44hAwYFUu4HzDKuGFmn7Ih0nd0u0ZtyAvrwmWuuakc6uIaipMiHjSB24t72UiJutrXvcFFKSERy9qY6/V5X7u18iw0VclCt3v2gFRP6X2K9ANs6mrpjFPTF5EPGbe9S3Hc/LVk/LKGYzejGDvw53/GLcWVXoOIdCehxNK7MftBJMl8Q6e6KmI+9X8ipwjS8uw8SFB+E0FWVWAiltOw2r8OLFUiQIzHWWTgBTWe4jCn5vcNwhUjDohY2RBaW+xZd5CtER6EQrPfbbotTw5X5kH1nvkey6+FEI3FUOOT9KtfOTGf+RxPjEhXJ4PfIFRSX2jz53TJWCoZ3NjgCOmhDQGpC/KQkFe+23kz7W3gM8LfcA+6H5APkjo8Ji3AcnE93ivZ7NHlP53Oof3VYurZl7yh/6PzmRI7y7IfqHR8qR+2uKkjC6v2nBvqQ== deploy"
      user_name = "pen_admin"
      sftp_id = dependency.transfer-sftp-dpxn-scan-ingestion.outputs.sftp_id
      transfer_server_role_arn = dependency.transfer-sftp-dpxn-scan-ingestion.outputs.sftp_transfer_server_iam_role
      tags = merge(dependency.common-tags.outputs.common_tags, {
        "TerraformPath" = path_relative_to_include()
      })
    },{
      s3_bucket_id = dependency.s3-dpxn-scan-ingestion.outputs.s3_bucket_id
      public_key = "ssh-rsa AAAAB3NzaC1yc2EAAAABEQAAAQEAvQtiKQKXtxtnX/n6l9VunJKQqEJg3/Ps+IUVSXivUj5AV/mtqvxJD3myXBx5iwFywrahjhWjcDgL7J7eicJWK2nYnsrK2YnxKqAZ+Z+DGAZeO3VKuFBJB2vWU/ySG25XhL7BypAa8w/bi6F8wpeRJly97zo5YZb0na+GgfCr3d2IIzageWELaW0WyOGYRzcux0k73/Llzunxuq9s3N3DzyoNMSABcle+qYwU8blA/eM7qSOKvhL2M9Psp2132Pf36wUOtOiGICtwNzjHt32m2h+5C1HZmDzTcC02p8WbYB8iAdRrospcLNkYLKV0PaiRlZ529pbHAbiWQ+96KKWx7w=="
      user_name = "global_scape"
      sftp_id = dependency.transfer-sftp-dpxn-scan-ingestion.outputs.sftp_id
      transfer_server_role_arn = dependency.transfer-sftp-dpxn-scan-ingestion.outputs.sftp_transfer_server_iam_role
      tags = merge(dependency.common-tags.outputs.common_tags, {
        "TerraformPath" = path_relative_to_include()
      })
    }]
}
