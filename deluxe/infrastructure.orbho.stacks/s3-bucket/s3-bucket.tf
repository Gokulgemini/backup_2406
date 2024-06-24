module "infra-bucket" {
  source = "git::ssh://git@bitbucket.org/deluxe-development/aws-s3-bucket.git?ref=2.1.5"

  bucket_name = var.bucket_name
  encryption_policy = false
  ssl_policy = false
  kms_master_key_id = var.kms_master_key_id
  versioning_enabled = true
  lifecycle_rule = var.lifecycle_rule
  
  tags = var.tags

  key    = var.key
  kms_key_id = var.kms_master_key_id
}

