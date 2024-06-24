module "alb" {
  source  =  "git::ssh://git@bitbucket.org/deluxe-development/aws-alb.git//?ref=1.0.0"

  alb_name               = var.alb_name
  vpc_id             = var.vpc_id
  alb_subnets            = var.alb_subnets
  alb_security_groups = var.alb_security_groups

  alb_target_groups = var.alb_target_groups

  http_tcp_listeners = var.http_tcp_listeners

  alb_tags = var.alb_tags
}