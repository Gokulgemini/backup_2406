locals {
  region_vars = read_terragrunt_config(find_in_parent_folders("region.hcl"))
  aws_region = local.region_vars.locals.aws_region
  environment_vars    = read_terragrunt_config(find_in_parent_folders("env.hcl"))


  # Define module source
  source_base_url = "git::ssh://git@bitbucket.org/deluxe-development/infrastructure.orboanywhereinfra.terraform.stacks.git//security-groups"
}

dependency "vpc-info" {
  config_path = "${get_terragrunt_dir()}/../vpc-info"

  mock_outputs_allowed_terraform_commands = ["validate","destroy", "plan-all", "plan"]
  mock_outputs = {
    vpc_id = "123"
  }
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  aws_region = local.aws_region
  vpc_id = dependency.vpc-info.outputs.vpc_id
  sg_tags = merge({
    TerraformPath = path_relative_to_include()}
  )
}
