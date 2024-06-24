# AWS Region
variable "region" {
  description = "Region in which AWS Resources to be created"
  type        = string
  default     = "us-west-1"
}
# Environment Variable
variable "environment" {
  description = "Environment Variable used as a prefix"
  type        = string
  default     = ""
  
}
# Business Division
variable "owners" {
  description = "organization this Infrastructure belongs"
  type        = string
  default     = "business Divison"
}

# VPC variables defined as below
# VPC Name
variable "name" {
  description = "VPC Name"
  type        = string
  default     = ""
}

# VPC CIDR Block
variable "cidr" {
  description = "VPC CIDR Block"
  type        = string
  default     = ""
}

# VPC Availability Zones
variable "azs" {
  description = "A list of availability zones names or ids in the region"
  type        = list(string)
  default     = null
}

# VPC Public Subnets
variable "public_subnets" {
  description = "A list of public subnets inside the VPC"
  type        = list(string)
  default     = null
}

# VPC Private Subnets
variable "private_subnets" {
  description = "A list of private subnets inside the VPC"
  type        = list(string)
  default     = null
}

# VPC Database Subnets
variable "database_subnets" {
  description = "A list of database subnets inside the VPC"
  type        = list(string)
  default     = null
}

# VPC Create Database Subnet Group (True / False)
variable "create_database_subnet_group" {
  description = "VPC Create Database Subnet Group"
  type        = bool
  default     = null
}

# VPC Create Database Subnet Route Table (True or False)
variable "create_database_subnet_route_table" {
  description = "VPC Create Database Subnet Route Table"
  type        = bool
  default     = null
}


# VPC Enable NAT Gateway (True or False) 
variable "enable_nat_gateway" {
  description = "provision NAT Gateways for each of your private networks"
  type        = bool
  default     = null
}

# VPC Single NAT Gateway (True or False)
variable "single_nat_gateway" {
  description = "single shared NAT Gateway across all of your private networks"
  type        = bool
  default     = null
}
