resource "time_static" "alb" {
  count = var.create_lb ? 1 : 0
}

resource "time_static" "asg" {
  count = (var.create_asg || var.create_ec2) ? 1 : 0
  triggers = {
    ami_id        = var.ami_filter_string != "" ? data.aws_ami.this[0].id : var.ami_id
    instance_type = var.instance_type
    user_data     = var.enable_domain_join ? base64encode(data.template_file.domain_join.rendered) : var.user_data_base64
  }
}

locals {
  alb_name = var.create_lb ? "${var.alb_name}-${time_static.alb[0].unix}" : var.alb_name
  lc_name  = var.create_lc ? "${var.lc_name}-${time_static.asg[0].unix}" : var.lc_name
  lt_name  = var.create_lt ? "${var.launch_template_name}-${time_static.asg[0].unix}" : var.launch_template_name
  asg_name = var.create_asg ? "${var.asg_name}-${time_static.asg[0].unix}" : var.asg_name

  lc_instance_type = var.create_asg && var.create_lc ? time_static.asg[0].triggers.instance_type : var.instance_type
  lc_ami_id        = var.create_asg && var.ami_id == "" ? time_static.asg[0].triggers.ami_id : var.ami_id

  lt_instance_type = var.create_asg && var.create_lt ? time_static.asg[0].triggers.instance_type : var.instance_type
  lt_ami_id        = var.create_asg && var.ami_id == "" ? time_static.asg[0].triggers.ami_id : var.ami_id

  ec2_ami_id = var.ami_id == "" ? data.aws_ami.this[0].id : var.ami_id

  key_name         = var.create_keypair ? module.key_pair.key_pair_key_name : var.instance_key_name
  user_data_base64 = var.enable_domain_join ? base64encode(time_static.asg[0].triggers.user_data) : var.user_data_base64

  target_group_arns = var.create_lb ? module.alb.target_group_arns : var.target_group_arns
}
