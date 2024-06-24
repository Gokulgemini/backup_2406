module "aws-sqs" {
  source = "git::ssh://git@bitbucket.org/deluxe-development/aws-sqs.git//?ref=1.0.0"
  name = var.name
  fifo_queue = var.fifo_queue
  content_based_deduplication = var.content_based_deduplication
  visibility_timeout_seconds = var.visibility_timeout_seconds
  delay_seconds = var.delay_seconds
  message_retention_seconds = var.message_retention_seconds
  max_message_size = var.max_message_size
  max_receive_count = var.max_receive_count
  kms_master_key_id = var.kms_master_key_id
  tags = var.tags
}
