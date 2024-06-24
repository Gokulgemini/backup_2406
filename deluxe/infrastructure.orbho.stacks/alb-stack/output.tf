output "loadbalancer_dns_name" {
  description = "The DNS name of the load balancer."
  value       = try(module.aws-alb.lb_dns_name, "")
}

output "loadbalancer_target_group_arns" {
  description = "ARNs of the target groups. Useful for passing to your Auto Scaling group."
  value       = try(module.aws-alb.target_group_arns, [])
}

output "lb_dns_name" {
  description = "Name of load balancer"
  value       = try(module.aws-alb.lb_dns_name, null)
}

output "target_group_arns" {
  description = "ARNs of the target groups"
  value       = try(module.aws-alb.target_group_arns, [])
}