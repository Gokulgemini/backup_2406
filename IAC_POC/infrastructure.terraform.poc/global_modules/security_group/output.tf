# Security Group
output "sg_ids" {
  description = "The ID of the Security Group"
  value = module.security_group
}