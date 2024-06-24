variable "bucket_name" {
  type = string
  description = "Name Of Bucket"
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

variable "kms_master_key_id" {
  type = string
  description = "KMS Master Key To Use"
  default = null
}

variable "sse_algorithm" {
  type = string
  description = "Encryption Algorithm to use."
  default = "aws:kms"
}

variable "encryption_policy" {
  type = bool
  default = true
  description = "Whether to set the encryption policy"
}

variable "ssl_policy" {
  type = bool
  default = true
  description = "Whether to set the ssl policy"
}

variable "tags" {
  type = map(string)
  description = "Tags to add to buckets"
}

variable "key" {
  type = string
  default = ""
  
}

variable "lifecycle_rule" {
  description = "S3 Bucket lifecycle rule configuration"
  type = any
  default = []
}