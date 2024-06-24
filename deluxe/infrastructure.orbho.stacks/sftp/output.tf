output "sftp_arn" {
  value = module.aws-sftp.sftp_arn
}

output "sftp_id" {
  value = module.aws-sftp.sftp_id
}

output "sftp_endpoint" {
  value = module.aws-sftp.sftp_endpoint
}

output "sftp_host_key_fingerprint" {
  value = module.aws-sftp.sftp_host_key_fingerprint
}

output "sftp_transfer_server_iam_role" {
  value = module.aws-sftp.sftp_transfer_server_iam_role
}

output "vpc_endpoint_id" {
  value = module.aws-sftp.vpc_endpoint_id
}

output "vpc_endpoint_dns" {
  value = module.aws-sftp.vpc_endpoint_dns
}
