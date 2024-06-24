resource "aws_lb" "apigw" {
  name               = "${var.rest_api_name}-lb"
  internal           = true
  load_balancer_type = "application"
  security_groups    = var.aws_apigw_vpce_sg_id
  subnets            = var.aws_apigw_vpce_subnets

  enable_deletion_protection = true
}

resource "aws_lb_target_group" "https_tg" { // Target Group HTTPS
 name     = "apigw-alb-https-tg"
 port     = 443
 protocol = "HTTPS"
 target_type = "ip"
 vpc_id   = var.vpc_id
}

resource "aws_lb_target_group" "http_tg" { // Target Group HTTP
 name     = "apigw-alb-http-tg"
 port     = 80
 protocol = "HTTP"
 target_type = "ip"
 vpc_id   = var.vpc_id
}

resource "aws_lb_target_group_attachment" "http_attach" {
 for_each = toset(var.target_group_ips)
 depends_on = [aws_vpc_endpoint.api-vpce]
 target_group_arn = aws_lb_target_group.http_tg.arn
 target_id        = each.value
 port             = 80
}

resource "aws_lb_target_group_attachment" "https_attach" {
 for_each = toset(var.target_group_ips)
 depends_on = [aws_vpc_endpoint.api-vpce]
 target_group_arn = aws_lb_target_group.https_tg.arn
 target_id        = each.value
 port             = 443
}

// Listener
resource "aws_lb_listener" "http_listener" {
 load_balancer_arn = aws_lb.apigw.arn
 port              = "80"
 protocol          = "HTTP"

 default_action {
   type             = "forward"
   target_group_arn = aws_lb_target_group.http_tg.arn
 }
}

// Listener
resource "aws_lb_listener" "https_listener" {
 load_balancer_arn = aws_lb.apigw.arn
 port              = "443"
 protocol          = "HTTPS"
 ssl_policy        = "ELBSecurityPolicy-2016-08"
 certificate_arn   = aws_acm_certificate.main.arn

 default_action {
   type             = "forward"
   target_group_arn = aws_lb_target_group.https_tg.arn
 }
}
