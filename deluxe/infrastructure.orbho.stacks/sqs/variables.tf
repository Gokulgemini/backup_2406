variable "name" {
  type = string
  default = null
}

variable "fifo_queue" {
  type = bool
  default = false
}

variable "content_based_deduplication" {
  type = bool
  default = false
}

variable "visibility_timeout_seconds" {
  type = number
  default = 600
}

variable "delay_seconds" {
  type = number
  default = 0
}

variable "message_retention_seconds" {
  type = number
  default = 1209600
}

variable "max_message_size" {
  type = number
  default = 262144
}

variable "max_receive_count" {
  type = number
  default = 1
}

variable "kms_master_key_id" {
  type = string
  default = null
}

variable "tags" {
  type        = map(any)
  default     = {}
  description = "Additional tags (e.g. map(`BusinessUnit`,`XYZ`)."
}
