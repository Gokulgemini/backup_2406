# Loadbalancer & Target Groups
variable "create_lb" {
  type        = bool
  description = "ALB conditional creation"
  default     = false
}

# Launch Configuration/Template
variable "create_lc" {
  description = "Determines whether to create launch configuration or not"
  type        = bool
  default     = false
}

variable "use_lc" {
  description = "Determines whether to use a launch configuration in the autoscaling group or not"
  type        = bool
  default     = false
}

variable "create_lt" {
  description = "Determines whether to create launch template or not"
  type        = bool
  default     = true
}

variable "use_lt" {
  description = "Determines whether to use a launch template in the autoscaling group or not"
  type        = bool
  default     = true
}

variable "launch_template_name" {
  description = "Name of launch template to be created"
  type        = string
  default     = null
}

variable "launch_template_description" {
  description = "Description of the launch template"
  type        = string
  default     = null
}

variable "ami_owner_id" {
  description = "Owner id for the AMI"
  type        = string
  default     = "095876105558"
}

variable "ami_filter_string" {
  description = "Filter string for finding suitable AMI"
  type        = string
  default     = "GS-DLX-WIN2022-BASELINE-*"
}

variable "instance_type" {
  description = "Instance Type To Use"
  type        = string
  default     = ""
}

variable "iam_instance_profile" {
  description = "The IAM instance profile to associate with launched instances"
  type        = string
  default     = ""
}

variable "instance_key_name" {
  description = "Instance Key Name to Inject to Instances"
  type        = string
  default     = ""
}

variable "instance_security_groups" {
  type        = list(string)
  description = "Array of security groups used for the ec2"
  default     = []
}

variable "block_device_mappings" {
  type        = list(any)
  description = "Custom block device mappings for launch templates"
  default     = []
}

# Autoscaling Groups / EC2 Instance Only
variable "create_ec2" {
  description = "Whether to create an instance"
  type        = bool
  default     = false
}

variable "create_asg" {
  description = "Determines whether to create autoscaling group or not"
  type        = bool
  default     = true
}

variable "asg_name" {
  description = "Name used for autoscaling groups"
  type        = string
  default     = ""
}

variable "max_size" {
  description = "Max Size for AutoScaling Group"
  type        = number
  default     = 2
}

variable "min_size" {
  description = "Minimum Size for AutoScaling Group"
  type        = number
  default     = 2
}

variable "desired_capacity" {
  description = "Desired Capacity for AutoScaling Group"
  type        = number
  default     = 2
}

variable "target_group_arns" {
  description = "A set of `aws_alb_target_group` ARNs, for use with Application or Network Load Balancing"
  type        = list(string)
  default     = []
}

variable "vpc_zone_identifier" {
  type        = list(string)
  description = "Array of subnets for ec2 instances to live in"
  default     = []
}

variable "enable_domain_join" {
  description = "Whether user data config is required for ec2 instances"
  type        = string
  default     = true
}

variable "directory_id" {
  description = "AWS active directory id which is used for domain join"
  type        = string
  default     = "d-9067fb338e"
}

variable "windows_version" {
  description = "Version of Windows Server that is being used (CloudCommon2016 or CloudCommon2019)"
  type        = string
  default     = "CloudCommon2022"
}

variable "monitoring" {
  description = "If true, the launched EC2 instance will have detailed monitoring enabled"
  type        = bool
  default     = true
}

variable "asg_tags" {
  type        = map(string)
  description = "Tags we want to put on asg resources"
  default     = {}
}
