output "queue_arn" {
  value = module.aws-sqs.queue.arn
}

output "queue_url" {
  value = module.aws-sqs.queue.id
}

output "queue" {
  value = module.aws-sqs.queue
}

output "deadletter_queue_arn" {
  value = module.aws-sqs.deadletter_queue.arn
}

output "deadletter_queue_id" {
  value = module.aws-sqs.deadletter_queue.id
}

output "deadletter_queue" {
  value = module.aws-sqs.deadletter_queue
}
