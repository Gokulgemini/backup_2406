locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))

  # Extract out common variables for reuse
  env = local.environment_vars.locals.chef_environment
  app_env = local.environment_vars.locals.app_environment
  modules_version = local.environment_vars.locals.dpxn_modules_version
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "git::git@github.com:verifyvalid/infrastructure.dpx_fulfillment.terraform.modules.git//s3-sftp-server-users?ref=${local.modules_version}"
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
    pen_sftp_id = "s-01234567890abcdef"
    pen_sftp_transfer_server_iam_role = "arn:aws:transfer:us-east-1:123456789012:server/s-01234567890abcdef"
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
      pen_sftp_id = dependency.transfer-sftp-dpxn-scan-ingestion.outputs.pen_sftp_id
      pen_transfer_server_role_arn = dependency.transfer-sftp-dpxn-scan-ingestion.outputs.pen_sftp_transfer_server_iam_role
      tags = merge(dependency.common-tags.outputs.common_tags, {
        "TerraformPath" = path_relative_to_include()
      })
    },{
      s3_bucket_id = dependency.s3-dpxn-scan-ingestion.outputs.s3_bucket_id
      public_key = "ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAABgQDVNPQrSGR4Op3Hyj5cpX0mNXizMjqFp5JXUm8U/F8imDJodI3w+zlq1J7BUh+afghnnmfiAkGHCeMP7wosMg6MbByZypltQTn0o36T1r53aD0ZXft9wYtLIAtj05oA9XVZm361Hw+dbtFQ6386f4mCvDolWMbDdvXBpgfXsnYCoidbdrKutpD4QDV/DxKp5cn3eoF8iT7yqiqitA8pKtYJfKluiIh67KTX9rvne0wHAwN4Cvj5BO5r3/cuiY2osLPA6y6u89IhZf1Nn5d4tODIgzmKRR2lqqqYO5+rxd9pi9hshM/SQ97K4OsH0jmzWO7eT8yOOMPnBR6bQQ0XzpVMFwwxZXN2z58Zu8eWlAakAsU/wzNj0sLEQQf7Gmh3zxPv36e6GtTP3tUzxMFuEjqzUDheWPVTNO2pP5lJuE7mutUa/9gI1ZMSVHUyhCfc0KxWv5wB/KOz5/VOI4Ld4yal+kd1QrjYMWRXc4h7M7c4N48BaSUa3lGwXLpFkElHIXU= ahallett@Aidans-MacBook-Pro.local"
      user_name = "aidan"
      pen_sftp_id = dependency.transfer-sftp-dpxn-scan-ingestion.outputs.pen_sftp_id
      pen_transfer_server_role_arn = dependency.transfer-sftp-dpxn-scan-ingestion.outputs.pen_sftp_transfer_server_iam_role
      tags = merge(dependency.common-tags.outputs.common_tags, {
        "TerraformPath" = path_relative_to_include()
      })
    },{
      s3_bucket_id = dependency.s3-dpxn-scan-ingestion.outputs.s3_bucket_id
      public_key = "ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAACAQDJFuNygJn/HGtooaOKIeZOoIrFh/asrne03NvgcuJe/vevqJbUtLejsCLHFHAaHkFwLMRtcYtED/oq2cgBjqxVtYWLz9SvbMObvPsGmwARKKQmetMb/Dy5IOTeNdxenljL3UmYwHlHoZYls3y86MmRJIWzTMLz4MLXRHAFvENz5gbog20Gh9rlASsGsJGKR4J7VNE7vfnNDckb71t1WZWzWygGuwBTcv3C49PUNDRtq3A7PR6BACq8rEgF9yWpjTmegiwq5gEEM9xA+ouaadow5izf+mq1GowHP8oVEEiiasuKGhm8oxac/tM3kjMB4Sz3sBt3p6V+xFib/uyRrUv7kiP2R/oMePeCWOD7FVT2/CVGBqhvQL1wfo5avIwhnCKYqJpRXpB72R/pwv+S/yUkZuKTYcDdYstmxEJ3ANfWxLZ1kVn4bj+l5bfFSlAFmhKerWxofT0J89v6Maz5kIeYUYVb88/WOXjmFGycnYx89BNZ8VlE3toi2ymytXOUNPz934NZroPJa1DwCFIQimyL4RmQehnOiT+nU474po/rY4qJqp3cWoC6o6N8ULpDvtnvOESzJu6GBN/lIKVF3qTyMKQrjt5F676336MjlnIIubshKVDbQreg0XLnM0Vem6YTqdF44FiZ0AHt+i4iOSsHbIp0zG+QOOHw+oCBkbZ+Hw== andrew.eruthayaraj@gmail.com"
      user_name = "andrew"
      pen_sftp_id = dependency.transfer-sftp-dpxn-scan-ingestion.outputs.pen_sftp_id
      pen_transfer_server_role_arn = dependency.transfer-sftp-dpxn-scan-ingestion.outputs.pen_sftp_transfer_server_iam_role
      tags = merge(dependency.common-tags.outputs.common_tags, {
        "TerraformPath" = path_relative_to_include()
      })
    },{
      s3_bucket_id = dependency.s3-dpxn-scan-ingestion.outputs.s3_bucket_id
      public_key = "ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAACAQDL1278KCQsEinKxv8n69BIk58XWj7XpQpmRkL/gwtmvArSQucWhY1+NzZb13O2rdMpTf75JI5rbPENbaCbToFXtDZ8/4por8Grh9IXbf00leu4hEXj22ToTIoElzuuE0CBx4n194gxRAah7xTit68HoH4h4QcQgjXMMi2ZYvOABoLgKqg7/ZybgaUaFfYLb3GPz/3WtzEGFkApf6moAKI15JYzmo0qM8759+f+xIONR1Mph48g7y66tRfeJP9AfXvXN2RIKnOgC4oY0FOx8Dcb3qYgI8BOZXj4gzS9lfKJZC2KhgEkNJt7BNG/gJ+Kt7owCzW5MmjbKfamapjzWb3xjluxP9AIcx7VCJadCIEic6ivIwWZNfy7fDkoNSuWqC9Bs7qISPDp7sF0A80BBu8a/+OmAOOa33ov5FTgpjD3SpxXTDxb5tzJr7DBbhbf5tS2jWNsFlpbBScaOb9kWwa3kECUv005ijE1ypItVzbJ5o6X58aGFtnyknUqlfpbvekOdgcOV5jStJLLNJFZE44j9BjnYj6eO7ETykzskwdPrl4C06yl0DXa1Kit1HDgBdSED1bkwInb5nnQCBh3+xWKD7I2mIAfiXKHljSc4JyRuJcyn7vLbS6uQo3R8otrXz57CAsQQBSyYNTTsWwBnIdCKboxf63gFVZRLG2h7dGSyw== sdevulkar@verticalresponse.com"
      user_name = "sri"
      pen_sftp_id = dependency.transfer-sftp-dpxn-scan-ingestion.outputs.pen_sftp_id
      pen_transfer_server_role_arn = dependency.transfer-sftp-dpxn-scan-ingestion.outputs.pen_sftp_transfer_server_iam_role
      tags = merge(dependency.common-tags.outputs.common_tags, {
        "TerraformPath" = path_relative_to_include()
      })
    },{
      s3_bucket_id = dependency.s3-dpxn-scan-ingestion.outputs.s3_bucket_id
      public_key = "ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAABAQD1eyvjCkg2rfEan5+lhdoR0p4bW7oEdhejfGLJ1jPl8B9Yi7KGjzTtc/8u+JSNJTAkSWXZGoir2NqVFykfzyHnhoKzuymD1K2Ase9YrpWuGm45rn4nM6I9VYV3iAkAr7tlkHwSYR/fR3G/jiPFppTs4mTKTTcvy2I0/AmVay42yTAfbrOOfTwQnnlXS84zUOt6HKQ9Efc7bwsKNmfsRVD0cARDGITgs/NEIMecqsrkvoxu/h9JEGQ/E5LuZGBj0xz+dVpj53/xRebkPGcM4DPJQnxhEwwsDOKMosO3YU1fiWS5hG5AGzmkxxh7iiqylVE9SDAhG+8p9lREJJVXuNOF jrider@host-159-6-0-10.sfoffice.verticalresponse.com"
      user_name = "joe"
      pen_sftp_id = dependency.transfer-sftp-dpxn-scan-ingestion.outputs.pen_sftp_id
      pen_transfer_server_role_arn = dependency.transfer-sftp-dpxn-scan-ingestion.outputs.pen_sftp_transfer_server_iam_role
      tags = merge(dependency.common-tags.outputs.common_tags, {
        "TerraformPath" = path_relative_to_include()
      })
     },{
      s3_bucket_id = dependency.s3-dpxn-scan-ingestion.outputs.s3_bucket_id
      public_key = "ssh-rsa AAAAB3NzaC1yc2EAAAABJQAAAQEAjiMI8iRA7VQhu3KtTSRMZHG0RaWgqL69NO/MM92+gU+BnQyINBhlnC2zuxOVAj557w/wtiAaZWISl8C6hoJ4v0oErigkK8C5+EfTt1J18uxfd+9NQR368flro+pZEQKBENFFAsOczkjstiJ9Z8miOWhS4tHQYRpdVwD/AJjcXT4nC9/yiWoX7ZdbcwdYR7QLbeAcSLNYzVNKwhBYww0jolQfsl298j5mThddFdBLghH2sXJ51ea8kFZw4y4eOLBmSZ+RnFpRRUmjEWFU6wAWCSXudqH0HlBrBX34WIJKf+rRHwrG6cIJAY4CkBHVMhtzxYn9O3mmKuuVmBG4RbQfjQ=="
      user_name = "eric"
      pen_sftp_id = dependency.transfer-sftp-dpxn-scan-ingestion.outputs.pen_sftp_id
      pen_transfer_server_role_arn = dependency.transfer-sftp-dpxn-scan-ingestion.outputs.pen_sftp_transfer_server_iam_role
      tags = merge(dependency.common-tags.outputs.common_tags, {
        "TerraformPath" = path_relative_to_include()
      })
    },{
      s3_bucket_id = dependency.s3-dpxn-scan-ingestion.outputs.s3_bucket_id
      public_key = "ssh-rsa AAAAB3NzaC1yc2EAAAABEQAAAQEAvQtiKQKXtxtnX/n6l9VunJKQqEJg3/Ps+IUVSXivUj5AV/mtqvxJD3myXBx5iwFywrahjhWjcDgL7J7eicJWK2nYnsrK2YnxKqAZ+Z+DGAZeO3VKuFBJB2vWU/ySG25XhL7BypAa8w/bi6F8wpeRJly97zo5YZb0na+GgfCr3d2IIzageWELaW0WyOGYRzcux0k73/Llzunxuq9s3N3DzyoNMSABcle+qYwU8blA/eM7qSOKvhL2M9Psp2132Pf36wUOtOiGICtwNzjHt32m2h+5C1HZmDzTcC02p8WbYB8iAdRrospcLNkYLKV0PaiRlZ529pbHAbiWQ+96KKWx7w=="
      user_name = "global_scape"
      pen_sftp_id = dependency.transfer-sftp-dpxn-scan-ingestion.outputs.pen_sftp_id
      pen_transfer_server_role_arn = dependency.transfer-sftp-dpxn-scan-ingestion.outputs.pen_sftp_transfer_server_iam_role
      tags = merge(dependency.common-tags.outputs.common_tags, {
        "TerraformPath" = path_relative_to_include()
      })
    }]
}
