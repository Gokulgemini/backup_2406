module "aws-alb" {
  source = "git::ssh://git@bitbucket.org/deluxe-development/aws-ec2-asg-alb.git//?ref=ec2-template"
# Load Balancers & Target Groups
  create_lc  = false
  create_lt  = false
  create_ec2 = false
  create_asg = false
  create_lb  = true 
  alb_name           = var.alb_name
  load_balancer_type = var.load_balancer_type
  vpc_id             = var.vpc_id
  alb_subnets        = var.alb_subnets
  idle_timeout       = var.idle_timeout
  alb_security_groups = var.alb_security_groups
  alb_target_groups = var.alb_target_groups
  http_tcp_listeners = var.http_tcp_listeners
  #https_listeners   = var.https_listeners
  alb_tags = var.alb_tags
}