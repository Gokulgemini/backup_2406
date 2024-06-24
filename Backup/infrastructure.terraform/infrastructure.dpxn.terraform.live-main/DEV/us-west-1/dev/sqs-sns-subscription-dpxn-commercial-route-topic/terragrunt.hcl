locals {
  modules_version = "1.0.0"
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "git::ssh://git@bitbucket.deluxe.com:7999/dtf/aws-sns-topic-subscription.git//?ref=${local.modules_version}"
}

# Include all settings from the root terragrunt.hcl file
include {
  path = find_in_parent_folders()
}

dependency "sns-dpxn-commercial-route-topic" {
  config_path = "${get_terragrunt_dir()}/../sns-dpxn-commercial-route-topic"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    sns_topic_arn = "arn:aws:sns:us-west-1:342434324183:topicName:23e2e27e-7bf0-4b83-9e56-4f6bdb53d2ab"
    sns_topic_id = "topic_id"
    sns_topic_name = "topic_name"
    sns_topic_owner = "topic_owner_account_id"
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

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  protocol = "sqs"
  sns_topic_arn = dependency.sns-dpxn-commercial-route-topic.outputs.sns_topic_arn
  target_endpoint_arn = [dependency.sqs-dpxn-commercial-route-events-queue.outputs.queue_arn]
}
