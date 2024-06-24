module "aws-ec2" {
  source = "git::ssh://git@bitbucket.org/deluxe-development/aws-ec2-asg-alb.git//?ref=orbograph_userdata_script"

  # Launch Template
  create_lc                   = false
  create_lt                   = true
  launch_template_name        = var.launch_template_name
  launch_template_description = var.launch_template_description
  ami_owner_id                = var.ami_owner_id
  ami_filter_string           = var.ami_filter_string
  instance_type               = var.instance_type
  instance_key_name           = var.instance_key_name
  iam_instance_profile        = var.iam_instance_profile
  enable_domain_join          = var.enable_domain_join
  directory_id                = var.directory_id
  windows_version             = var.windows_version
  instance_security_groups    = var.instance_security_groups
  block_device_mappings       = var.block_device_mappings
  monitoring                  = var.monitoring
  # Autoscaling Groups
  create_lb           = false
  create_ec2          = false
  create_asg          = true
  use_lc              = false
  use_lt              = true
  asg_name            = var.asg_name
  min_size            = var.min_size
  max_size            = var.max_size
  desired_capacity    = var.desired_capacity
  target_group_arns   = var.target_group_arns
  vpc_zone_identifier = var.vpc_zone_identifier
  asg_tags            = merge({
      key                 = "Domain"
      value               = "deluxe.com"
      propagate_at_launch = true
    },
    {
      key                 = "Application"
      value               = "devops-orbo"
      propagate_at_launch = true
    },
    {
      key                 = "Owner"
      value               = "OrboGraph"
      propagate_at_launch = true
    },
    {
      key                 = "OSType"
      value               = "Windows"
      propagate_at_launch = true
    },
    {
      key                 = "OSFlavor"
      value               = "Windows Server 2022"
      propagate_at_launch = true
    },var.asg_tags)
}
