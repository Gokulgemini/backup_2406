locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))

  # Extract out common variables for reuse
  infra_environment = lower(replace(local.environment_vars.locals.infra_environment,"_", "-"))
  app_environment = lower(local.environment_vars.locals.app_environment)
  modules_version = "1.2.3"
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "git::git@bitbucket.org:deluxe-development/aws-iam-role-with-policy.git//?ref=${local.modules_version}"
}

# Include all settings from the root terragrunt.hcl file
include {
  path = find_in_parent_folders()
}

dependency "s3-dpxn-commercial-router" {
  config_path = "${get_terragrunt_dir()}/../s3-dpxn-commercial-router"

  mock_outputs_allowed_terraform_commands = ["validate","plan"]
  mock_outputs = {
    s3_bucket_id = "key_id"
    s3_bucket_arn = "arn::"
    s3_bucket_domain_name = "dsf"
  }
}

dependency "sqs-dpxn-commercial-router" {
  config_path = "${get_terragrunt_dir()}/../sqs-dpxn-commercial-router"

  mock_outputs_allowed_terraform_commands = ["validate","plan"]
  mock_outputs = {
    queue_arn = "arn::"
    queue_id = "12344"
    queue = "queue"
    deadletter_queue_arn = "arn::"
    deadletter_queue_id = "12344"
    deadletter_queue = "d_queue"
  }
}

dependency "sqs-dpxn-commercial-route-events-queue" {
  config_path = "${get_terragrunt_dir()}/../sqs-dpxn-commercial-route-events-queue"

  mock_outputs_allowed_terraform_commands = ["validate","plan"]
  mock_outputs = {
    queue_arn = "arn::"
    queue_id = "12344"
    queue = "queue"
    deadletter_queue_arn = "arn::"
    deadletter_queue_id = "12344"
    deadletter_queue = "d_queue"
  }
}

dependency "kms-dpxn-commercial-router" {
  config_path = "${get_terragrunt_dir()}/../kms-dpxn-commercial-router"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    kms_arn = "arn::"
    kms_id = "key_id"
    kms_key_alias_arn = "arn::"
  }
}

dependency "kms-dpxn-payment-router" {
  config_path = "${get_terragrunt_dir()}/../kms-dpxn-payment-router"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    kms_arn = "arn::"
    kms_id = "key_id"
    kms_key_alias_arn = "arn::"
  }
}

dependency "kms-dpxn-payment-events" {
  config_path = "${get_terragrunt_dir()}/../kms-dpxn-payment-events"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    kms_arn = "arn::"
    kms_id = "key_id"
    kms_key_alias_arn = "arn::"
  }
}

dependency "logstream-dpxn-commercial-router-cloudwatch-log-group" {
  config_path = "${get_terragrunt_dir()}/../../_global/vpc-info"

  mock_outputs_allowed_terraform_commands = ["validate","plan"]
  mock_outputs = {
    cloudwatch_log_group_arn = "arn::"
  }
}

dependency "sns-dpxn-payment-router-responder-events" {
  config_path = "${get_terragrunt_dir()}/../sns-dpxn-payment-router-responder-events"

  mock_outputs_allowed_terraform_commands = ["validate","plan"]
  mock_outputs = {
    sns_topic_id = "topic_id"
    sns_topic_arn = "arn::"
    sns_topic_name = "topic_name"
    sns_topic_owner = "root"
  }
}

dependency "sns-dpxn-payment-events" {
  config_path = "${get_terragrunt_dir()}/../sns-dpxn-payment-events"

  mock_outputs_allowed_terraform_commands = ["validate","plan"]
  mock_outputs = {
    sns_topic_id = "topic_id"
    sns_topic_arn = "arn::"
    sns_topic_name = "topic_name"
    sns_topic_owner = "root"
  }
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
  oidc_provider = true
  create_role = true
  create_policy = true
  role_name = "${local.infra_environment}-${local.app_environment}-commercial-router-role"
  role_description = "Role used by DLX Commercial Router Service (${local.app_environment})"
  policy_name = "${local.infra_environment}-${local.app_environment}-commercial-router-policy"
  policy_description = "Policy used by DLX Commercial Router Role (${local.app_environment})"
  role_path = "/"
  policy_statement = templatefile("policy/statement.tftpl", {
           sns-dpxn-payment-router-responder-events-topic-arn = dependency.sns-dpxn-payment-router-responder-events.outputs.sns_topic_arn,
           sns-dpxn-payment-events-topic-arn = dependency.sns-dpxn-payment-events.outputs.sns_topic_arn,
           sqs-dpxn-commercial-router-queue-arn = dependency.sqs-dpxn-commercial-router.outputs.queue_arn,
           sqs-dpxn-commercial-route-events-queue-arn = dependency.sqs-dpxn-commercial-route-events-queue.outputs.queue_arn,
           s3-dpxn-commercial-router-bucket-arn = dependency.s3-dpxn-commercial-router.outputs.s3_bucket_arn,
					 kms-dpxn-commercial-router-key-arn = dependency.kms-dpxn-commercial-router.outputs.kms_arn,
           kms-dpxn-payment-router-key-arn = dependency.kms-dpxn-payment-router.outputs.kms_arn,
           kms-dpxn-payment-events-key-arn = dependency.kms-dpxn-payment-events.outputs.kms_arn,
					 logstream-dpxn-commercial-router-cloudwatch-log-group-arn = dependency.logstream-dpxn-commercial-router-cloudwatch-log-group.outputs.cloudwatch_log_group_arn})
  oidc_fully_qualified_subjects = [
    "system:serviceaccount:payment-routing:dlx-commercial-router"
  ]
  oidc_fully_qualified_audiences = [
    "sts.amazonaws.com"
  ]
  provider_url = dependency.eks-fargate-cluster.outputs.oidc_role_url

  tags = {
    "TerraformPath" = path_relative_to_include()
  }
}
