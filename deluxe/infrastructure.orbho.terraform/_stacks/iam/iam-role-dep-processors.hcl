locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))
  region_vars      = read_terragrunt_config(find_in_parent_folders("region.hcl"))

  # Extract out common variables for reuse
  infra_environment = lower(replace(local.environment_vars.locals.infra_environment, "_", "-"))
  app_environment   = lower(local.environment_vars.locals.app_environment)
  aws_region        = lower(local.region_vars.locals.aws_region)

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

dependency "sqs-dep-orbo-queue" {
  config_path = "${get_terragrunt_dir()}/../sqs-dep-orbo-queue"

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

dependency "s3-dep-results" {
  config_path = "${get_terragrunt_dir()}/../s3-dep-results"

  mock_outputs_allowed_terraform_commands = ["validate", "plan"]
  mock_outputs = {
    s3_bucket_id          = "key_id"
    s3_bucket_arn         = "arn::"
    s3_bucket_domain_name = "dsf"
  }
}

dependency "s3-dep-documents" {
  config_path = "${get_terragrunt_dir()}/../s3-dep-documents"

  mock_outputs_allowed_terraform_commands = ["validate", "plan"]
  mock_outputs = {
    s3_bucket_id          = "key_id"
    s3_bucket_arn         = "arn::"
    s3_bucket_domain_name = "dsf"
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

dependency "kms-dep-orbo-services" {
  config_path = "${get_terragrunt_dir()}/../kms-dep-orbo-services"

  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy"]
  mock_outputs = {
    kms_arn           = "arn::"
    kms_id            = "key_id"
    kms_key_alias_arn = "arn::"
  }
}

dependency "kms-dep-documents" {
  config_path = "${get_terragrunt_dir()}/../kms-dep-documents"

  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy"]
  mock_outputs = {
    kms_arn           = "arn::"
    kms_id            = "key_id"
    kms_key_alias_arn = "arn::"
  }
}

dependency "kms-dep-results" {
  config_path = "${get_terragrunt_dir()}/../kms-dep-results"

  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy"]
  mock_outputs = {
    kms_arn           = "arn::"
    kms_id            = "key_id"
    kms_key_alias_arn = "arn::"
  }
}

dependency "kms-db-key" {
  config_path = "${get_terragrunt_dir()}/../../_global/kms-db-key"

  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy"]
  mock_outputs = {
    kms_arn           = "arn::"
    kms_id            = "key_id"
    kms_key_alias_arn = "arn::"
  }
}

dependency "dep-results-dynamodb" {
  config_path = "${get_terragrunt_dir()}/../dep-results-dynamodb"
 
  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy"]
  mock_outputs = {
    dynamodb_table_arn           = "arn::"
    dynamodb_table_id            = "key_id"
  }
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  role_name          = "${local.infra_environment}-${local.aws_region}-dep-processors-role"
  role_description   = "Role used by DEP Processors (${local.app_environment})"
  policy_name        = "${local.infra_environment}-${local.aws_region}-dep-processors-policy"
  policy_description = "Policy used by DEP Processors Role (${local.app_environment})"
  managed_policy_arns = ["arn:aws:iam::aws:policy/AmazonSSMManagedInstanceCore","arn:aws:iam::851725215854:policy/EC2_Full_Access_Policy"]
  policy_statement = templatefile("policy-processors/statement.tftpl", {
    sqs-dep-batch-queue-arn       = dependency.sqs-dep-batch-queue.outputs.queue_arn,
    sqs-dep-orbo-queue-arn        = dependency.sqs-dep-orbo-queue.outputs.queue_arn,
    s3-dep-results-bucket-arn     = dependency.s3-dep-results.outputs.s3_bucket_arn,
    s3-dep-documents-bucket-arn   = dependency.s3-dep-documents.outputs.s3_bucket_arn,
    kms-dep-processors-key-arn    = dependency.kms-dep-processors.outputs.kms_arn,
    kms-dep-orbo-services-key-arn = dependency.kms-dep-orbo-services.outputs.kms_arn
    kms-dep-documents-key-arn     = dependency.kms-dep-documents.outputs.kms_arn,
    kms-dep-results-key-arn       = dependency.kms-dep-results.outputs.kms_arn
    dep-results-dynamodb-table-arn = dependency.dep-results-dynamodb.outputs.dynamodb_table_arn,
    kms-db-key-key-arn            = dependency.kms-db-key.outputs.kms_arn})
  trusted_entities = [
    "ec2.amazonaws.com"
  ]
  tags = {
    "TerraformPath" = path_relative_to_include()
  }
}
