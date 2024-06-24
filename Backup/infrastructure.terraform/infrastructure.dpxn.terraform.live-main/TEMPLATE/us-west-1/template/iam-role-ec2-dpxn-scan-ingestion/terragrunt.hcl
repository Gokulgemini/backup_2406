locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))
  account_vars = read_terragrunt_config(find_in_parent_folders("account.hcl"))
  account_name = lower(local.account_vars.locals.account_name)

  # Extract out common variables for reuse
  env = local.environment_vars.locals.chef_environment
  app_environment = lower(local.environment_vars.locals.app_environment)
  modules_version = "1.2.4"
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "git::ssh://git@bitbucket.deluxe.com:7999/dtf/aws-iam-role-with-policy.git//?ref=${local.modules_version}"

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

dependency "s3-dpxn-interim" {
  config_path = "${get_terragrunt_dir()}/../s3-dpxn-interim"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    s3_bucket_id = "key_id"
    s3_bucket_arn = "arn::"
    s3_bucket_domain_name = "dsf"
  }
}

dependency "s3-dpxn-scan-ingestion" {
  config_path = "${get_terragrunt_dir()}/../s3-dpxn-scan-ingestion"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    s3_bucket_id = "key_id"
    s3_bucket_arn = "arn::"
    s3_bucket_domain_name = "dsf"
  }
}

dependency "s3-dpxn-acknowledgement" {
  config_path = "${get_terragrunt_dir()}/../s3-dpxn-acknowledgement"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    s3_bucket_id = "key_id"
    s3_bucket_arn = "arn::"
    s3_bucket_domain_name = "dsf"
  }
}

dependency "sqs-dpxn-check-print" {
  config_path = "${get_terragrunt_dir()}/../sqs-dpxn-check-print"

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

dependency "sqs-dpxn-scan-ingestion-nonfifo" {
  config_path = "${get_terragrunt_dir()}/../sqs-dpxn-scan-ingestion-nonfifo"

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

dependency "sqs-dpxn-acknowledgement" {
  config_path = "${get_terragrunt_dir()}/../sqs-dpxn-acknowledgement"

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

dependency "kms-non-db-key" {
  config_path = "${get_terragrunt_dir()}/../../_global/kms-non-db-key"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    kms_arn = "arn::"
    kms_id = "key_id"
    kms_key_alias_arn = "arn::"
  }
}

dependency "kms-dpxn-scan-ingestion" {
  config_path = "${get_terragrunt_dir()}/../kms-dpxn-scan-ingestion"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    kms_arn = "arn::"
    kms_id = "key_id"
    kms_key_alias_arn = "arn::"
  }
}

dependency "kms-dpxn-acknowledgement" {
  config_path = "${get_terragrunt_dir()}/../kms-dpxn-acknowledgement"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    kms_arn = "arn::"
    kms_id = "key_id"
    kms_key_alias_arn = "arn::"
  }
}

dependency "kms-dpxn-check-print" {
  config_path = "${get_terragrunt_dir()}/../kms-dpxn-check-print"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    kms_arn = "arn::"
    kms_id = "key_id"
    kms_key_alias_arn = "arn::"
  }
}

dependency "kms-db-key" {
  config_path = "${get_terragrunt_dir()}/../kms-db-key"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    kms_arn = "arn::"
    kms_id = "key_id"
    kms_key_alias_arn = "arn::"
  }
}

dependency "s3-infra-bucket" {
  config_path = "${get_terragrunt_dir()}/../../_global/s3-infra-bucket"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    s3_bucket_id = "key_id"
    s3_bucket_arn = "arn::"
    s3_bucket_domain_name = "dsf"
  }
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  create_role = true
  create_instance_profile = true
  create_policy = true
  role_name = "dpxn-${local.env}-${local.app_environment}-scan-ingestion"
  role_description = "Role used by dpxn scan ingestion service (${local.app_environment})"
  policy_name = "${local.env}-${local.app_environment}-scan-ingestion-policy"
  policy_description = "Used for the Scan Ingestion service (${local.app_environment})"
  policy_statement = <<EOF
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Action": [
        "sqs:DeleteMessage",
        "sqs:ReceiveMessage",
        "sqs:SendMessage"
      ],
      "Effect": "Allow",
      "Resource": "${dependency.sqs-dpxn-scan-ingestion-nonfifo.outputs.queue_arn}"
    },
    {
      "Action": [
        "sqs:DeleteMessage",
        "sqs:ReceiveMessage"
      ],
      "Effect": "Allow",
      "Resource": "${dependency.sqs-dpxn-acknowledgement.outputs.queue_arn}"
    },
    {
      "Action": [
        "sqs:DeleteMessage",
        "sqs:ReceiveMessage",
        "sqs:SendMessage"
      ],
      "Effect": "Allow",
      "Resource": "${dependency.sqs-dpxn-check-print.outputs.queue_arn}"
    },
    {
      "Action": [
        "s3:GetObject",
        "s3:GetObjectAcl"
      ],
      "Effect": "Allow",
      "Resource": "${dependency.s3-dpxn-scan-ingestion.outputs.s3_bucket_arn}/*"
    },{
      "Action": [
        "s3:GetObject",
        "s3:GetObjectAcl",
        "s3:PutObject",
        "s3:PutObjectAcl"
      ],
      "Effect": "Allow",
      "Resource": "${dependency.s3-dpxn-interim.outputs.s3_bucket_arn}/*"
    },{
      "Action": [
        "s3:GetObject",
        "s3:GetObjectAcl"
      ],
      "Effect": "Allow",
      "Resource": "${dependency.s3-dpxn-acknowledgement.outputs.s3_bucket_arn}/*"
    },{
      "Effect": "Allow",
      "Action": [
        "s3:GetObject",
        "s3:GetObjectAcl",
        "kms:Decrypt"
      ],
      "Resource": [
        "${dependency.s3-infra-bucket.outputs.s3_bucket_arn}/*",
        "${dependency.s3-infra-bucket.outputs.s3_bucket_arn}/"
      ]
    },{
      "Action": [
        "kms:Encrypt",
        "kms:Decrypt",
        "kms:GenerateDataKey",
        "kms:DescribeKey",
        "kms:ReEncryptFrom",
        "kms:ReEncryptTo"
      ],
      "Effect": "Allow",
      "Resource": [
        "${dependency.kms-db-key.outputs.kms_arn}",
        "${dependency.kms-non-db-key.outputs.kms_arn}",
        "${dependency.kms-dpxn-scan-ingestion.outputs.kms_arn}",
        "${dependency.kms-dpxn-check-print.outputs.kms_arn}",
        "${dependency.kms-dpxn-acknowledgement.outputs.kms_arn}"
      ]
    },
    {
      "Sid": "AccessForCloudwatch",
      "Effect": "Allow",
      "Action": [
        "cloudwatch:ListMetrics",
        "cloudwatch:PutMetricData",
        "cloudwatch:GetMetricStatistics",
        "ec2:DescribeInstances"
      ],
      "Resource": [
        "*"
      ]
    },
    {
      "Sid": "AccessForCloudwatchLogs",
      "Effect": "Allow",
      "Action": [
        "ec2:DescribeInstances",
        "ec2:DescribeTags",
        "logs:CreateLogGroup",
        "logs:CreateLogStream",
        "logs:DescribeLogStreams",
        "logs:PutLogEvents"
      ],
      "Resource": [
        "*"
      ]
    }
  ]
}
EOF
  attach_managed_policy = true
  managed_policy_arns = [
    "arn:aws:iam::aws:policy/AmazonSSMManagedInstanceCore"
  ]
  trusted_entities = [
    "ec2.amazonaws.com"
  ]
  oidc_provider = false
}
