# Loadbalancer & Target Groups
variable "create_lb" {
  type        = bool
  description = "ALB conditional creation"
  default     = true
}

variable "alb_name" {
  description = "The resource name and Name tag of the load balancer."
  type        = string
  default     = "alb"
}

variable "load_balancer_type" {
  type        = string
  description = "The Type of load balancer"
  default     = "application"
}

variable "vpc_id" {
  type        = string
  description = "The ID of the VPC we are being created in"
  default     = ""
}

variable "alb_subnets" {
  type        = list(string)
  description = "Array of the Subnets we should be using with ALB"
  default     = []
}

variable "alb_security_groups" {
  type        = list(string)
  description = "Array of security groups used for the load balancer"
  default     = []
}

variable "alb_target_groups" {
  type        = list(any)
  description = "The target groups config"
  default     = []
}

variable "http_tcp_listeners" {
  type        = list(any)
  description = "http_tcp_listeners configuration"
  default     = []
}

variable "https_listeners" {
  type        = list(any)
  description = "https_listeners configuration"
  default     = []
}

variable "https_listener_rules" {
  description = "A list of maps describing the Listener Rules for this ALB. Required key/values: actions, conditions. Optional key/values: priority, https_listener_index (default to https_listeners[count.index])"
  type        = list(any)
  default     = []
}

variable "http_tcp_listener_rules" {
  description = "A list of maps describing the Listener Rules for this ALB. Required key/values: actions, conditions. Optional key/values: priority, http_tcp_listener_index (default to http_tcp_listeners[count.index])"
  type        = list(any)
  default     = []
}

variable "idle_timeout" {
  description = "The time in seconds that the connection is allowed to be idle."
  type        = number
  default     = 60
}

variable "alb_tags" {
  description = "A map of tags to add to loadbalancer and target groups"
  type        = map(string)
  default     = {}
}

# Launch Configuration/Template
variable "create_lc" {
  description = "Determines whether to create launch configuration or not"
  type        = bool
  default     = true
}

variable "use_lc" {
  description = "Determines whether to use a launch configuration in the autoscaling group or not"
  type        = bool
  default     = true
}

variable "lc_name" {
  description = "Name of launch configuration to be created"
  type        = string
  default     = "lc"
}

variable "create_lt" {
  description = "Determines whether to create launch template or not"
  type        = bool
  default     = false
}

variable "use_lt" {
  description = "Determines whether to use a launch template in the autoscaling group or not"
  type        = bool
  default     = false
}

variable "launch_template_name" {
  description = "Name of launch template to be created"
  type        = string
  default     = "lt"
}

variable "launch_template_description" {
  description = "Description of the launch template"
  type        = string
  default     = null
}

variable "launch_template_version" {
  description = "Launch template version. Can be version number, `$Latest`, or `$Default`"
  type        = string
  default     = null
}

variable "ebs_optimized" {
  description = "Whether we should enable ebs_optimized"
  type        = bool
  default     = true
}

variable "ami_id" {
  description = "Image Id we will use with servers"
  type        = string
  default     = ""
}

variable "ami_owner_id" {
  description = "Owner id for the AMI"
  type        = string
  default     = ""
}

variable "ami_filter_string" {
  description = "Filter string for finding suitable AMI"
  type        = string
  default     = ""
}

variable "instance_type" {
  description = "Instance Type To Use"
  type        = string
  default     = "t4g.micro"
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

variable "user_data_base64" {
  description = "The Base64-encoded user data to provide when launching the instance"
  type        = string
  default     = ""
}

variable "user_data_replace_on_change" {
  description = "When used in combination with user_data or user_data_base64 will trigger a destroy and recreate when set to true. Defaults to false if not set."
  type        = bool
  default     = false
}

variable "instance_security_groups" {
  type        = list(string)
  description = "Array of security groups used for the ec2"
  default     = []
}

variable "associate_public_ip_address" {
  description = "(LC) Associate a public ip address with an instance in a VPC"
  type        = bool
  default     = false
}

variable "ebs_block_device" {
  type        = list(map(string))
  description = "Additional EBS block devices to attach to the instance"
  default     = []
}

variable "root_block_device" {
  type        = list(any)
  description = "Customize details about the root block device of the instance"
  default     = []
}

variable "block_device_mappings" {
  type        = list(any)
  description = "Custom block device mappings for launch templates"
  default     = []
}

variable "network_interfaces" {
  type        = list(map(string))
  description = "Customize network interfaces to be attached at instance boot time"
  default     = []
}

# Autoscaling Groups / EC2 Instance Only
variable "create_ec2" {
  description = "Whether to create an instance"
  type        = bool
  default     = false
}

variable "create_keypair" {
  description = "Whether to create ec2 key pair"
  type        = bool
  default     = false
}

variable "instance_name" {
  description = "Name to be used on EC2 instance created"
  type        = string
  default     = ""
}

variable "create_asg" {
  description = "Determines whether to create autoscaling group or not"
  type        = bool
  default     = true
}

variable "asg_name" {
  description = "Name used for autoscaling groups"
  type        = string
  default     = "asg"
}

variable "max_size" {
  description = "Max Size for AutoScaling Group"
  type        = number
  default     = 1
}

variable "min_size" {
  description = "Minimum Size for AutoScaling Group"
  type        = number
  default     = 1
}

variable "desired_capacity" {
  description = "Desired Capacity for AutoScaling Group"
  type        = number
  default     = 1
}

variable "default_cooldown" {
  description = "Default Cooldown for AutoScaling Group"
  type        = number
  default     = 300
}

variable "health_check_type" {
  type        = string
  description = "Type of Health Check"
  default     = "EC2"
}

variable "subnet_id" {
  description = "The VPC Subnet ID to launch in"
  type        = string
  default     = ""
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
  default     = false
}

variable "directory_id" {
  description = "AWS active directory id which is used for domain join"
  type        = string
  default     = ""
}

variable "windows_version" {
  description = "Version of Windows Server that is being used (CloudCommon2016 or CloudCommon2019)"
  type        = string
  default     = "CloudCommon2019"
}

variable "monitoring" {
  description = "If true, the launched EC2 instance will have detailed monitoring enabled"
  type        = bool
  default     = false
}

variable "instance_tags" {
  description = "A mapping of tags to assign to the devices created by the instance at launch time"
  type        = map(string)
  default     = {}
}

variable "volume_tags" {
  description = "A mapping of tags to assign to the devices created by the instance at launch time"
  type        = map(string)
  default     = {}
}

variable "enable_volume_tags" {
  description = "Whether to enable volume tags (if enabled it conflicts with root_block_device tags)"
  type        = bool
  default     = false
}

variable "asg_tags" {
  type        = map(string)
  description = "Tags we want to put on asg resources"
  default     = {}
}

variable "timeouts" {
  description = "Define maximum timeout for creating, updating, and deleting EC2 instance resources"
  type        = map(string)
  default     = {}
}

variable "alb_access_logs" {
  description = "Map containing access logging configuration for load balancer"
  type        = map(string)
  default     = {}
}