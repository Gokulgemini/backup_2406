module "s3_bucket" {
  source = "terraform-aws-modules/s3-bucket/aws"
  version = "v3.2.1"
 
  bucket = var.bucket_name
  acl = var.acl
  grant = var.grant
  lifecycle_rule = var.lifecycle_rule
 
  versioning = {
    enabled = var.versioning_enabled
  }
 
 
  # S3 bucket-level Public Access Block configuration
  block_public_acls       = var.block_public_acls
  block_public_policy     = var.block_public_policy
  ignore_public_acls      = var.ignore_public_acls
  restrict_public_buckets = var.restrict_public_buckets
 

 
}
 
module "object" {
  source = "terraform-aws-modules/s3-bucket/aws//modules/object"
  create = var.create_object
 
  bucket     = module.s3_bucket.s3_bucket_id
  key        = var.key
  #kms_key_id = var.kms_key_id
}