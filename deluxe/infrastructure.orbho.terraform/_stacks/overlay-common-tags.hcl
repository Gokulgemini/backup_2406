locals {
  # Automatically load environment-level variables
  environment_vars    = read_terragrunt_config(find_in_parent_folders("env.hcl"))
  overlay_common_tags = local.environment_vars.locals.env_tags

  # Extract out common variables for reuse
  source_base_url   = "git::ssh://git@bitbucket.org/deluxe-development/aws-common-tags.git//"
}

dependency "common-tags" {
  config_path = "${get_terragrunt_dir()}/../../_global/common-tags"

  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy"]
  mock_outputs = {
    asg_common_tags = [
      {
        key                 = "AccountId"
        propagate_at_launch = true
        value               = "3432"
      }
    ]
    common_tags = {
      "AppName" = "DEP"
    }
  }
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  env_tags = merge(dependency.common-tags.outputs.common_tags, local.overlay_common_tags)
}
