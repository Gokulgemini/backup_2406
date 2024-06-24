variable "s3_bucket_id" {
  type = string
  description = "Name of the bucket to point too"
  default = ""
}

variable "endpoint_type" {
  type = string
  description = "The endpoint type we are using"
  default = ""
}

variable "identity_provider_type" {
  type        = string
  default     = ""
  description = "The mode of authentication enabled for this service. The default value is SERVICE_MANAGED, which allows you to store and access SFTP user credentials within the service. API_GATEWAY."
}

variable "tags" {
  type        = map(any)
  default     = {}
  description = "Additional tags (e.g. map(`BusinessUnit`,`XYZ`)."
}

variable "vpc_id" {
  type        = string
  default     = ""
  description = "VPC ID"
}

variable "iam_role_name" {
  type        = string
  default     = ""
  description = "Iam Role name (sftp service)"
}

variable "kms_key_arn" {
  type        = string
  description = "KMS Key To Use"
  default     = ""
}

variable "subnet_ids" {
  type = list(string)
  default = ["d1","f2"]
}

variable "vpce_security_group_id" {
  type      = string
  default = ""
  description = "Security group to assign to VPC endpoint"
}

