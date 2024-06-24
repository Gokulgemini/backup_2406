locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))

  # Extract out common variables for reuse
  chef_environment = lower(replace(local.environment_vars.locals.chef_environment, "_", "-"))
  app_environment  = lower(local.environment_vars.locals.app_environment)
  modules_version  = "1.2.4"
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "git::git@bitbucket.org:deluxe-development/aws-iam-role-with-policy.git//?ref=${local.modules_version}"

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

    arguments = [
      "-var-file=${get_terragrunt_dir()}/${path_relative_from_include()}/common.tfvars",
    ]
  }
}

# Include all settings from the root terragrunt.hcl file
include {
  path = find_in_parent_folders()
}

dependency "eks-fargate-cluster" {
  config_path = "${get_terragrunt_dir()}/../eks-fargate-cluster"
  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    fargate_role_arn = "arn::"
    cluster_name = "cluster"
    oidc_role_url = "oidc.eks.us-west-1.amazonaws.com/id/eeff"
  }
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  oidc_provider           = true
  create_role             = true
  create_policy           = true
  role_name               = "${local.chef_environment}-${local.app_environment}-print-scan-order-events-role"
  role_description        = "Role used by DPXN Print Scan Order Events Service (${local.app_environment})"
  policy_name             = "${local.chef_environment}-${local.app_environment}-print-scan-order-events-policy"
  policy_description      = "Policy used by DPXN Print Scan Order Events Service Role (${local.app_environment})"
  policy_statement        = templatefile("policy/statement.tftpl", {})
  oidc_fully_qualified_subjects   = [
    "system:serviceaccount:print-scan-service-layer:dlx-print-scan-order-events"
  ]
  oidc_fully_qualified_audiences  = [
    "sts.amazonaws.com"
  ]
  provider_url            = dependency.eks-fargate-cluster.outputs.oidc_role_url
  tags                    = {
    "TerraformPath" = path_relative_to_include()
  }
}
