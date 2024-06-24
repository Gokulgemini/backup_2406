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

dependency "s3-dpxn-anomalies" {
  config_path = "${get_terragrunt_dir()}/../s3-dpxn-anomalies"

  mock_outputs_allowed_terraform_commands = ["validate","plan"]
  mock_outputs = {
    s3_bucket_id = "key_id"
    s3_bucket_arn = "arn::"
    s3_bucket_domain_name = "dsf"
  }
}

dependency "s3-dpxn-payment-router-input" {
  config_path = "${get_terragrunt_dir()}/../s3-dpxn-payment-router-input"

  mock_outputs_allowed_terraform_commands = ["validate","plan"]
  mock_outputs = {
    s3_bucket_id = "key_id"
    s3_bucket_arn = "arn::"
    s3_bucket_domain_name = "dsf"
  }
}

dependency "s3-dpxn-payment-router-plugins" {
  config_path = "${get_terragrunt_dir()}/../s3-dpxn-payment-router-plugins"

  mock_outputs_allowed_terraform_commands = ["validate","plan"]
  mock_outputs = {
    s3_bucket_id = "key_id"
    s3_bucket_arn = "arn::"
    s3_bucket_domain_name = "dsf"
  }
}

dependency "sqs-dpxn-anomalies" {
  config_path = "${get_terragrunt_dir()}/../sqs-dpxn-anomalies"

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

dependency "sqs-dpxn-payment-router-input" {
  config_path = "${get_terragrunt_dir()}/../sqs-dpxn-payment-router-input"

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

dependency "sqs-dpxn-payment-router-responder-events" {
  config_path = "${get_terragrunt_dir()}/../sqs-dpxn-payment-router-responder-events"

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

dependency "s3-dpxn-commercial-router" {
  config_path = "${get_terragrunt_dir()}/../s3-dpxn-commercial-router"

  mock_outputs_allowed_terraform_commands = ["validate","plan"]
  mock_outputs = {
    s3_bucket_id = "key_id"
    s3_bucket_arn = "arn::"
    s3_bucket_domain_name = "dsf"
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

dependency "vpc-info" {
  config_path = "${get_terragrunt_dir()}/../../_global/vpc-info"

  mock_outputs_allowed_terraform_commands = ["validate","plan"]
  mock_outputs = {
    cloudwatch_log_group_arn = "arn::"
  }
}

dependency "eks-fargate-cluster" {
  config_path                             = "${get_terragrunt_dir()}/../eks-fargate-cluster"
  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy"]
  mock_outputs = {
    fargate_role_arn = "arn::"
    cluster_name     = "cluster"
    oidc_role_url    = "oidc.eks.us-west-1.amazonaws.com/id/eeff"
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

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  oidc_provider = true
  create_role = true
  create_policy = true
  role_name = "${local.infra_environment }-${local.app_environment}-payment-router-role"
  role_description = "Role used for the Payment Router service (${local.app_environment})"
  policy_name = "${local.infra_environment }-${local.app_environment}-payment-router-policy"
  policy_description = "Used for the Payment Router service (${local.app_environment})"
  role_path = "/"
  policy_statement = templatefile("policy/statement.tftpl", { sns-dpxn-payment-events-topic-arn = dependency.sns-dpxn-payment-events.outputs.sns_topic_arn,
                      sqs-dpxn-anomalies-queue-arn = dependency.sqs-dpxn-anomalies.outputs.queue_arn,
                      sqs-dpxn-payment-router-input-queue-arn = dependency.sqs-dpxn-payment-router-input.outputs.queue_arn,
                      sqs-dpxn-payment-router-responder-events-arn = dependency.sqs-dpxn-payment-router-responder-events.outputs.queue_arn,
                      sqs-dpxn-anomalies-queue-arn = dependency.sqs-dpxn-anomalies.outputs.queue_arn,
                      sqs-dpxn-payment-router-input-queue-arn = dependency.sqs-dpxn-payment-router-input.outputs.queue_arn,
                      s3-dpxn-anomalies-bucket-arn = dependency.s3-dpxn-anomalies.outputs.s3_bucket_arn,
                      s3-dpxn-payment-router-input-bucket-arn =   dependency.s3-dpxn-payment-router-input.outputs.s3_bucket_arn,
                      s3-dpxn-router-plugins-bucket-arn = dependency.s3-dpxn-payment-router-plugins.outputs.s3_bucket_arn,
                      s3-dpxn-commercial-router = dependency.s3-dpxn-commercial-router.outputs.s3_bucket_arn,
                      kms-dpxn-commercial-router = dependency.kms-dpxn-commercial-router.outputs.kms_arn,
                      kms-s3-payment-router-kms-arn = dependency.kms-dpxn-payment-router.outputs.kms_arn,
                      kms-sqs-payment-router-kms-arn = dependency.kms-dpxn-payment-router.outputs.kms_arn,
                      kms-dpxn-payment-events-key-arn = dependency.kms-dpxn-payment-events.outputs.kms_arn,
                      kms-dpxn-payment-router-key-arn = dependency.kms-dpxn-payment-router.outputs.kms_arn,
                      logstream-router-cloudwatch-log-group-arn = dependency.vpc-info.outputs.cloudwatch_log_group_arn
  })
  oidc_fully_qualified_subjects = [
    "system:serviceaccount:payment-routing:dlx-payment-router"
  ]
  oidc_fully_qualified_audiences = [
    "sts.amazonaws.com"
  ]
  provider_url = dependency.eks-fargate-cluster.outputs.oidc_role_url

   tags = {
    "TerraformPath" = path_relative_to_include()
  }

}
