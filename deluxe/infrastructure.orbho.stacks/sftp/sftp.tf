module "aws-sftp" {
  source = "git::ssh://git@bitbucket.org/deluxe-development/aws-sftp.git//?ref=feature/sg-id-argument-bug-fix"
  s3_bucket_id           = var.s3_bucket_id
  identity_provider_type = "SERVICE_MANAGED"
  vpc_id                 = var.vpc_id
  subnet_ids             = var.subnet_ids
  vpce_security_group_id = var.vpce_security_group_id
  endpoint_type          = "VPC"
  iam_role_name          = var.iam_role_name
  kms_key_arn            = var.kms_key_arn
  tags                   = var.tags
}