locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))

  # Extract out common variables for reuse
  env = local.environment_vars.locals.infra_environment
  app_environment = lower(local.environment_vars.locals.app_environment)
  module_version = "2.4.5"
  app_name = "dpxn"
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "git::git@bitbucket.org:deluxe-development/aws-eks-fargate-cluster.git//?ref=${local.module_version}"


  extra_arguments "common_var" {
    commands = [
      "apply",
      "apply-all",
      "plan-all",
      "destroy-all",
      "plan",
      "import",
      "push",
      "refresh"
    ]
  }
}

dependency "common-tags" {
  config_path = "${get_terragrunt_dir()}/../overlay-common-tags"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    common_tags = {
      "AppName" = "DPXNGA"
    }
  }
}

dependency "vpc-info" {
  config_path = "${get_terragrunt_dir()}/../../_global/vpc-info"

  mock_outputs_allowed_terraform_commands = ["validate","destroy", "plan-all"]
  mock_outputs = {
    vpc_id = "123"
  }
}

# Include all settings from the root terragrunt.hcl file
include {
  path = find_in_parent_folders()
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  subnet_ids = dependency.vpc-info.outputs.app_subnets
  cluster_vpc_id = dependency.vpc-info.outputs.vpc_id
  app_environment = local.app_environment
  k8s_version = "1.26"
  load_balancer_controller_version = "1.4.8"
  cluster_name = "${local.app_name}-fargate" //Note: module adds app_env prefix to cluster_name
  remove_default_aws_auth = true             //Remove default aws_auth created by fargate profile
  endpoint_private_access = true
  endpoint_public_access = true
  enable_aws_observability = false
  tags = merge({ "TerraformPath" = path_relative_to_include() }, dependency.common-tags.outputs.common_tags)
}
