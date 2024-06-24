module "alb" {
  source  = "terraform-aws-modules/alb/aws"
  version = "~> 6.7.0"

  create_lb = var.create_lb

  name               = local.alb_name
  load_balancer_type = var.load_balancer_type
  vpc_id             = var.vpc_id
  subnets            = var.alb_subnets
  security_groups    = var.alb_security_groups
  internal           = true
  idle_timeout       = var.idle_timeout
  access_logs        = var.alb_access_logs

  target_groups = var.alb_target_groups

  http_tcp_listeners = var.http_tcp_listeners

  https_listeners = var.https_listeners

  http_tcp_listener_rules = var.http_tcp_listener_rules

  https_listener_rules = var.https_listener_rules

  tags = var.alb_tags
}
