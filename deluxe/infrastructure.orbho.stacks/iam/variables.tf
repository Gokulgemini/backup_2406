# IAM Roles & Policies
variable "create_role" {
  description = "Whether to create an IAM Role"
  type        = bool
  default     = true
}

variable "create_instance_profile" {
  description = "Whether to create an Instance profile"
  type        = bool
  default     = true
}

variable "create_policy" {
  description = "Whether to create IAM Policy"
  type        = bool
  default     = true
}

variable "role_name" {
  description = "Name of the IAM Role"
  type        = string
}

variable "role_path" {
  description = "Path of the IAM role"
  type        = string
  default     = "/"
}

variable "role_description" {
  description = "The description of the IAM Role"
  type        = string
  default     = null
}

variable "policy_name" {
  description = "Name of the IAM Policy"
  type        = string
}

variable "policy_description" {
  description = "The description of the IAM Policy"
  type        = string
  default     = null
}

variable "policy_statement" {
  description = "Contents of policy statement"
  type        = string
  default     = ""
}

variable "attach_managed_policy" {
  description = "Whether to attach existing IAM Policies"
  type        = bool
  default     = true
}

variable "managed_policy_arns" {
  description = "List of IAM Policies to be attached"
  type        = list(string)
  default     = []
}

variable "trusted_entities" {
  description = "List of IAM trusted services to be attached"
  type        = list(string)
  default     = []
}

variable "tags" {
  description = "A map of tags to add to IAM role resources"
  type        = map(string)
  default     = {}
}
