# Route53 Zone vars

variable "create_zone" {
  description = "Whether to create Route53 zone"
  type        = bool
  default     = false
}

# Route53 Records vars
variable "create_public_record" {
  description = "Whether to create public DNS records"
  type        = bool
  default     = true
}

variable "create_private_record" {
  description = "Whether to create private DNS records"
  type        = bool
  default     = true
}

variable "zone_ids" {
  description = "All zone ids public and private (only if applicable)"
  type        = map(string)
  default     = {}
}

variable "records_jsonencoded" {
  description = "List of map of DNS records (stored as jsonencoded string, for terragrunt)"
  type        = string
  default     = null
}

# Generic vars
variable "tags" {
  description = "Tags added to all zones. Will take precedence over tags from the 'zones' variable"
  type        = map(string)
  default     = {}
}
