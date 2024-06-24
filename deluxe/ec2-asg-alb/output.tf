output "loadbalancer_dns_name" {
  description = "The DNS name of the load balancer."
  value       = try(module.alb.lb_dns_name, "")
}

output "loadbalancer_target_group_arns" {
  description = "ARNs of the target groups. Useful for passing to your Auto Scaling group."
  value       = try(module.alb.target_group_arns, [])
}

output "autoscaling_group_name" {
  description = "List of autoscaling group names."
  value       = try(module.asg.autoscaling_group_name, "")
}

output "launch_template_id" {
  description = "The ID of the launch template"
  value       = try(module.asg.launch_template_id, "")
}

output "launch_template_latest_version" {
  description = "The latest version of the launch template"
  value       = try(module.asg.launch_template_latest_version, "")
}

output "instance_id" {
  description = "The id of the instance."
  value       = try(aws_instance.this[0].id, "")
}

output "private_ip_dns_name" {
  description = "The dns name of the instance."
  value       = try(aws_instance.this[0].private_dns, "")
}

output "private_ip" {
  description = "The private IP  of the instance."
  value       = try(aws_instance.this[0].private_ip, "")
}

output "key_pair_name" {
  description = "The key pair name."
  value       = try(module.key_pair.key_pair_name, var.instance_key_name)
}

output "key_pair_private_key" {
  description = "The key pair private key content."
  value       = try(tls_private_key.this[0].private_key_openssh, "")
  sensitive   = true
}

output "lb_dns_name" {
  description = "Name of load balancer"
  value       = try(module.alb.lb_dns_name, null)
}

output "target_group_arns" {
  description = "ARNs of the target groups"
  value       = try(module.alb.target_group_arns, [])
}