variable "ami_filter_string" {
  description = "Filter string for finding suitable AMI"
  type        = string
  default     = ""
}

variable "ami_id" {
  description = "Image Id we will use with servers"
  type        = string
  default     = null
}

variable "ami_owner_id" {
  description = "Owner id for the AMI"
  type        = string
  default     = ""
}

variable "associate_public_ip_address" {
  description = "(LC) Associate a public ip address with an instance in a VPC"
  type        = bool
  default     = false
}

variable "create_keypair" {
  description = "Whether to create ec2 key pair"
  type        = bool
  default     = null
}

variable "ebs_block_device" {
  type        = list(map(any))
  description = "Additional EBS block devices to attach to the instance"
  default     = [{}]
}

variable "iam_instance_profile" {
  description = "The IAM instance profile to associate with launched instances"
  type        = string
  default     = ""
}

variable "instance_name" {
  description = "Name to be used on EC2 instance created"
  type        = string
  default     = null
}

variable "instance_type" {
  description = "Instance Type To Use"
  type        = string
  default     = ""
}

variable "instance_key_name" {
  description = "Instance Key Name to Inject to Instances"
  type        = string
  default     = ""
}

variable "instance_tags" {
  description = "A mapping of tags to assign to the devices created by the instance at launch time"
  type        = map(string)
  default     = {}
}

variable "monitoring" {
  description = "If true, the launched EC2 instance will have detailed monitoring enabled"
  type        = bool
  default     = null
}

variable "private_ip" {
  description = "Private IP address to associate with the instance in a VPC"
  type        = string
  default     = null
}

variable "vpc_security_group_ids" {
  description = "A list of security group IDs to associate with"
  type        = string
  default     = ""
}

variable "server_hostname" {
  description = "Variable to change the hostname of the Windows Server"
  type        = string
  default     = ""
}

variable "subnet_id" {
  description = "The VPC Subnet ID to launch in"
  type        = string
  default     = ""
}

variable "timeouts" {
  description = "Define maximum timeout for creating, updating, and deleting EC2 instance resources"
  type        = map(string)
  default     = {}
}

variable "volume_tags" {
  description = "A mapping of tags to assign to the devices created by the instance at launch time"
  type        = map(string)
  default     = {}
}

variable "zone_ids" {
  description = "Ids of Route53 zones you want the instance added too."
  type        = map(string)
  default     = null
}

variable "region" {
  description = "Region in which AWS Resources to be created"
  type        = string
  default     = null
}