variable "sg_name" {
  description = "Name of the security group"
  type = string
  default = null
}

variable "vpc_id" {
  description = "VPC id for security group"
  type = string
  default = null
}

variable "ingress_cidr_block" {
  description = "Ingress CIDR block for security group"
  type = list(string)
  default = null
}

variable "ingress_rule" {
  description = "Ingress rules for security group"
  type = list(string)
  default = null
}

variable "from_port" {
  description = "From port for ingress"
  type = number
  default = null
}

variable "to_port" {
  description = "To port for ingress"
  type = number
  default = null
}

variable "protocal" {
  description = "protocal for security group"
  type = string
  default = null
}
variable "cidr_block" {
  description = "CIDR range for ingress"
  type = string
  default = null
}