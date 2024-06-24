variable "bucket_name" {
  type = string
  description = "Name Of Bucket"
  default = "iac-poc-test-bucket-msys"
}
 
variable "acl" {
  type = string
  description = "Bucket ACL"
  default = null
}
 
variable "versioning_enabled" {
  type = bool
  description = "Enable Versioning for Bucket"
  default = true
}

 
variable "block_public_acls" {
  type = bool
  default = true
}
 
variable "block_public_policy" {
  type = bool
  default = true
}
 
variable "ignore_public_acls" {
  type = bool
  default = true
}
 
variable "restrict_public_buckets" {
  type = bool
  default = true
}
 
variable "lifecycle_rule" {
  description = "S3 Bucket lifecycle rule configuration"
  type = any
  default = [
    {
      id          = "expire_versions"
      enabled     = true
      expiration  = {
        days = 30
      }
      noncurrent_version_expiration = [
        {
          newer_noncurrent_versions = null  # No limit on number of newer noncurrent versions
          noncurrent_days           = 30
        }
      ]
    }
  ]
}
 
variable "grant" {
  description = "An ACL policy grant. Conflicts with `acl`"
  type        = any
  default     = []
}
 
variable "key" {
  type = string
  description = "Bucket folder structure"
  default = false
}
 
variable "create_object" {
  type = bool
  description = "Enable Objects in Bucket"
  default = false
}
