output "security_group_ids" {
  value = module.security_group.*.id
}