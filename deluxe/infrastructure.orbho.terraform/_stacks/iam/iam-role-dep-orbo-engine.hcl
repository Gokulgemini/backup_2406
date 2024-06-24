locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))
  account_vars     = read_terragrunt_config(find_in_parent_folders("account.hcl"))
  region_vars      = read_terragrunt_config(find_in_parent_folders("region.hcl"))

  # Extract out common variables for reuse
  app_environment   = lower(local.environment_vars.locals.app_environment)
  infra_environment = lower(local.account_vars.locals.account_name)
  aws_region        = lower(local.region_vars.locals.aws_region)

  source_base_url   = "git::ssh://git@bitbucket.org/deluxe-development/infrastructure.orboanywhereinfra.terraform.stacks.git//iam"
}

dependency "kms-dep-orbo-services" {
  config_path = "${get_terragrunt_dir()}/../kms-dep-orbo-services"

  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy"]
  mock_outputs = {
    kms_arn           = "arn::"
    kms_id            = "key_id"
    kms_key_alias_arn = "arn::"
  }
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  role_name          = "${local.infra_environment}-${local.aws_region}-dep-orbo-engine-role"
  role_description   = "Role used by DEP Orbo Engine (${local.app_environment})"
  policy_name        = "${local.infra_environment}-${local.aws_region}-dep-orbo-engine-policy"
  policy_description = "Policy used by DEP Orbo Engine Role (${local.app_environment})"
  managed_policy_arns = ["arn:aws:iam::aws:policy/AmazonSSMManagedInstanceCore","arn:aws:iam::851725215854:policy/EC2_Full_Access_Policy"]
  policy_statement   = templatefile("policy-engine/statement.tftpl", { kms-dep-orbo-services-key-arn   = dependency.kms-dep-orbo-services.outputs.kms_arn })
  trusted_entities = [
    "ec2.amazonaws.com"
  ]
  tags = {
    "TerraformPath" = path_relative_to_include()
  }
}
