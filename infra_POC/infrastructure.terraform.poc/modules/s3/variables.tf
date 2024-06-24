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
  default = []
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
