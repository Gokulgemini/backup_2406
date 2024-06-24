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

dependency "s3-dpxn-acknowledgement" {
  config_path = "${get_terragrunt_dir()}/../s3-dpxn-acknowledgement"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    s3_bucket_id = "key_id"
    s3_bucket_arn = "arn::"
    s3_bucket_domain_name = "dsf"
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

dependency "s3-dpxn-interim" {
  config_path = "${get_terragrunt_dir()}/../s3-dpxn-interim"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    s3_bucket_id = "key_id"
    s3_bucket_arn = "arn::"
    s3_bucket_domain_name = "dsf"
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

dependency "kms-db-key" {
  config_path = "${get_terragrunt_dir()}/../kms-db-key"

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

dependency "kms-dpxn-commercial-router" {
  config_path = "${get_terragrunt_dir()}/../kms-dpxn-commercial-router"

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

dependency "sns-dpxn-commercial-route-topic" {
  config_path = "${get_terragrunt_dir()}/../sns-dpxn-commercial-route-topic"
  mock_outputs_allowed_terraform_commands = ["validate","plan"]
  mock_outputs = {
    sns_topic_arn = "arn::"
    sns_topic_id = "topic_id"
	sns_topic_name = "topic_name"
	sns_topic_owner = "root"
  }
}

dependency "common-tags" {
  config_path = "${get_terragrunt_dir()}/../overlay-common-tags"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy", "plan-all"]
  mock_outputs = {
    asg_common_tags = [
      {
        key = "AccountId"
        propagate_at_launch = true
        value = "3432"
      }
    ]
    common_tags = {
      "AppName" = "DPXN"
    }
  }
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  create_role = true
  create_instance_profile = true
  create_policy = true
  role_name = "${local.env}-${local.app_environment}-ack_worker-role"
  role_description = "Role used by dpxn ack worker service (${local.app_environment})"
  policy_name = "${local.env}-${local.app_environment}-ack-worker-policy"
  policy_description = "Used for the Acknowledgement Worker service (${local.app_environment})"
  policy_statement = <<EOF
{
  "Version": "2012-10-17",
  "Statement": [
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
        "s3:GetObject",
        "s3:GetObjectAcl"
      ],
      "Effect": "Allow",
      "Resource": "${dependency.s3-dpxn-acknowledgement.outputs.s3_bucket_arn}/*"
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
        "${dependency.kms-dpxn-commercial-router.outputs.kms_arn}",
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
    },
    {
      "Sid": "AccessForSNS",
      "Effect": "Allow",
      "Action": [
        "sns:Publish"
      ],
      "Resource": [
        "${dependency.sns-dpxn-commercial-route-topic.outputs.sns_topic_arn}"
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
  tags = "${dependency.common-tags.outputs.common_tags}"
  oidc_provider = false
}
