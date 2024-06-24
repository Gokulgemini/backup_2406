module "asg" {
  source  = "terraform-aws-modules/autoscaling/aws"
  version = "~> 5.0.0"

  # Launch Configuration/Template
  create_lc = var.create_lc
  create_lt = var.create_lt

  lc_name            = local.lc_name
  lc_use_name_prefix = false

  lt_use_name_prefix = false
  lt_name            = local.lt_name
  description        = var.launch_template_description

  ebs_optimized             = var.ebs_optimized
  image_id                  = try(local.lc_ami_id, local.lt_ami_id)
  instance_type             = try(local.lc_instance_type, local.lt_instance_type)
  iam_instance_profile_name = var.iam_instance_profile
  key_name                  = var.instance_key_name
  user_data_base64          = local.user_data_base64

  security_groups             = var.instance_security_groups
  associate_public_ip_address = var.associate_public_ip_address

  ebs_block_device  = var.ebs_block_device
  root_block_device = var.root_block_device

  block_device_mappings = var.block_device_mappings
  enable_monitoring     = var.monitoring
  network_interfaces    = var.network_interfaces

  # Autoscaling Group
  create_asg = var.create_asg

  use_lc = var.use_lc
  use_lt = var.use_lt

  launch_configuration = try(module.asg.launch_configuration_name, var.lc_name)
  launch_template      = var.launch_template_name
  lt_version           = try(module.asg.launch_template_latest_version, var.launch_template_version)

  name             = local.asg_name
  use_name_prefix  = false
  min_size         = var.max_size
  max_size         = var.min_size
  desired_capacity = var.desired_capacity

  health_check_type = var.health_check_type
  default_cooldown  = var.default_cooldown

  target_group_arns   = local.target_group_arns
  vpc_zone_identifier = var.vpc_zone_identifier

  tags = var.asg_tags
}
