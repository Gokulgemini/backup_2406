variable "name" {
  type        = string
  description = "resource name, do not include environment or region"
  default = ""
}
variable "environment" {
  type        = string
  description = "Environment to be used"
  default = ""
}
variable "region" {
  type        = string
  description = "Region used on the naming convention"
  default = ""
}
variable "hash_key" {
  type        = string
  default     = null
  description = "(Optional) The attribute to use as hash (partition) key. Must be defined on the attribute variable"
}
variable "attributes" {
  type        = list(map(string))
  default     = []
  description = "(Optional) List of nested attributes for hash_key and range_key"
}

variable "point_in_time_recovery_enabled" {
  type        = bool
  default     = true
  description = "(Optional) Whether to enable point in time recovery"
}
variable "range_key" {
  type        = string
  default     = null
  description = "(sort) key. Must be defined on the attributes variable"
}

variable "table_class" {
  type        = string
  default     = null
  description = "(Optional) The storage class of the table. Valid values are STANDARD and STANDARD_INFREQUENT_ACCESS"
}
variable "tags" {
  type        = map(string)
  default     = {}
  description = "(Optional) a set of tags to add to all resources"
}


variable "ttl_attribute_name" {
  type        = string
  default     = ""
  description = "(Optional) The name of the attribute to store the TTL timestamp in"
}

variable "replica_regions" {
  type        = any
  default     = []
  description = "(Optioanl)Region names for global tables replicas"
}

