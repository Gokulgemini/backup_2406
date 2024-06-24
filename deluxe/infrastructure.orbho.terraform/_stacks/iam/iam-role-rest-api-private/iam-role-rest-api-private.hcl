locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))
  account_vars     = read_terragrunt_config(find_in_parent_folders("account.hcl"))

  # Extract out common variables for reuse
  app_environment   = lower(local.environment_vars.locals.app_environment)
  infra_environment = lower(local.account_vars.locals.account_name)

  source_base_url   = "git::ssh://git@bitbucket.org/deluxe-development/infrastructure.orboanywhereinfra.terraform.stacks.git//iam"
}

dependency "sqs-dep-batch-queue" {
  config_path = "${get_terragrunt_dir()}/../sqs-dep-batch-queue"

  mock_outputs_allowed_terraform_commands = ["validate", "plan"]
  mock_outputs = {
    queue_arn            = "arn::"
    queue_id             = "12344"
    queue                = "queue"
    deadletter_queue_arn = "arn::"
    deadletter_queue_id  = "12344"
    deadletter_queue     = "d_queue"
  }
}

dependency "kms-dep-processors" {
  config_path = "${get_terragrunt_dir()}/../kms-dep-processors"

  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy"]
  mock_outputs = {
    kms_arn           = "arn::"
    kms_id            = "key_id"
    kms_key_alias_arn = "arn::"
  }
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  create_role        = true
  create_policy      = true
  role_name          = "dep-${local.infra_environment}-rest-api-orbo-recognition"
  role_description   = "Role used by rest-api-orbo-recognition (${local.app_environment})"
  policy_name        = "dep-${local.infra_environment}-rest-api-orbo-recognition"
  policy_description = "Policy used by rest-api-orbo-recognition (${local.app_environment})"
  role_path          = "/"
  managed_policy_arns = ["arn:aws:iam::aws:policy/service-role/AmazonAPIGatewayPushToCloudWatchLogs"]
  policy_statement   = templatefile("policy/statement.tftpl", { 
    sqs-dep-batch-queue-arn       = dependency.sqs-dep-batch-queue.outputs.queue_arn,
    kms-dep-processors-key-arn    = dependency.kms-dep-processors.outputs.kms_arn
  })
  trusted_entities = [
    "apigateway.amazonaws.com"
  ]
  tags = {
    "TerraformPath" = path_relative_to_include()
  }
}

