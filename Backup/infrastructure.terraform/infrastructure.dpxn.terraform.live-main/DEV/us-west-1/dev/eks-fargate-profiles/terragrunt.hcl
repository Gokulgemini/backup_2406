locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))

  # Extract out common variables for reuse
  env = local.environment_vars.locals.infra_environment
  app_environment = lower(local.environment_vars.locals.app_environment)
  module_version = "2.0.0"
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "git::git@bitbucket.org:deluxe-development/aws-eks-fargate-profile.git//?ref=${local.module_version}"


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

dependency "eks-fargate-cluster" {
  config_path = "${get_terragrunt_dir()}/../eks-fargate-cluster"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    fargate_role_arn = "arn::"
    eks_cluster_name = "cluster"
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
  cluster_name = dependency.eks-fargate-cluster.outputs.eks_cluster_name
  fargate_profiles = [{
    fargate_profile_name = "${dependency.eks-fargate-cluster.outputs.eks_cluster_name}-External",
    fargate_role_arn = dependency.eks-fargate-cluster.outputs.fargate_role_arn,
    subnet_ids = dependency.vpc-info.outputs.app_subnets,
    namespace = "external"
  },{
    fargate_profile_name = "${dependency.eks-fargate-cluster.outputs.eks_cluster_name}-FileIngestion",
    fargate_role_arn = dependency.eks-fargate-cluster.outputs.fargate_role_arn,
    subnet_ids = dependency.vpc-info.outputs.app_subnets,
    namespace = "file-ingestion"
  },{
    fargate_profile_name = "${dependency.eks-fargate-cluster.outputs.eks_cluster_name}-PaymentRouting",
    fargate_role_arn = dependency.eks-fargate-cluster.outputs.fargate_role_arn,
    subnet_ids = dependency.vpc-info.outputs.app_subnets,
    namespace = "payment-routing"
  },{
    fargate_profile_name = "${dependency.eks-fargate-cluster.outputs.eks_cluster_name}-Reporting",
    fargate_role_arn = dependency.eks-fargate-cluster.outputs.fargate_role_arn,
    subnet_ids = dependency.vpc-info.outputs.app_subnets,
    namespace = "reporting"
  },{
    fargate_profile_name = "${dependency.eks-fargate-cluster.outputs.eks_cluster_name}-S4ServiceLayer",
    fargate_role_arn = dependency.eks-fargate-cluster.outputs.fargate_role_arn,
    subnet_ids = dependency.vpc-info.outputs.app_subnets,
    namespace = "print-scan-service-layer"
  }]
}
