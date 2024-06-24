# Security Group
variable "security_groups" {
  description = "Map of objects that define the security groups to be created."
  type = list(object({
    identifier     = string # Resource identifier of the Security Group
    sg_name        = string # Name of the Security Group
    sg_description = string # Short description of the Security Group
    vpc_id         = string # ID of the VPC where to create Security Group
    ingress_with_cidr_blocks = list(object({
      from_port   = number
      to_port     = number
      protocol    = string
      description = string
      cidr_blocks = string
    })) # Inbound rules for the Security Group
    egress_with_cidr_blocks = list(object({
      from_port   = number
      to_port     = number
      protocol    = string
      description = string
      cidr_blocks = string
    }))                   # # Outbound rules for the Security Group
    ingress_with_source_security_group_id = list(object({
      from_port   = number
      to_port     = number
      protocol    = string
      description = string
      source_security_group_id = string
    }))

    sg_tags = map(string) # Resource tags for the Security Group
  }))
}

variable "create_timeout" {
  description = "Time to wait for a security group to be created"
  type        = string
  default     = "15m"
}

variable "delete_timeout" {
  description = "Time to wait for a security group to be deleted"
  type        = string
  default     = "20m"
}
