variable "acm_tags" {
  description = "Tags for ACM Certificates"
  type        = map(string)
  default     = {}
}

variable "cert_bucket_name" {
  description = "Bucket name where SSL certificates are stored"
  type        = string
  default     = ""
}

variable "environment" {
  description = "Environment name"
  type        = string
  default     = ""
}

variable "pem_password" {
  type        = string
  description = "Password used for PEM key"
}

variable "pfx_object" {
  type        = string
  description = "Name of PFX Certificate needing to be uploaded to ACM"
}

variable "pfx_password" {
  type        = string
  description = "Password for PFX Certificate needing to uploaded to ACM"
}