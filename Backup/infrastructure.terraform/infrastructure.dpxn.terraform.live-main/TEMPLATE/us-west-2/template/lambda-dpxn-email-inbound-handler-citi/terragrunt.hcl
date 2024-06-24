locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))

  # Extract out common variables for reuse
  env = local.environment_vars.locals.chef_environment
  app_env = local.environment_vars.locals.app_environment
  modules_version = local.environment_vars.locals.dpxn_modules_version
  customer = "citi"
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "git::git@github.com:verifyvalid/infrastructure.dpx_fulfillment.terraform.modules.git//lambda-ses-email-handler?ref=${local.modules_version}"
}

# Include all settings from the root terragrunt.hcl file
include {
  path = find_in_parent_folders()
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
  function_name = "${local.env}-${local.app_env}-dpxn-email-inbound-handler"
  memory_size = 256
  policy_statement = {}
  tags = merge(dependency.common-tags.outputs.common_tags, { "TerraformPath" = path_relative_to_include() } )
  approved_email_list = "<ENTER LIST HERE>"
  approved_email_domains = "<ENTER DOMAIN LIST HERE>"
  api_key_name = "x-key-${local.customer}"
  # You will need to pass in api_key or input the value when it asks you for it. For lower environments you can provide it in the
  # inputs to avoid having to provide on deploy time.
  api_key = "<ENTER KEY HERE>"
}
