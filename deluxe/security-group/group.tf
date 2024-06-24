# Create AWS Security Group with Inbound & Outbound Rules. 
module "sg" {
  source = "terraform-aws-modules/security-group/aws"
  version = "~> 4.0"

  for_each = { for group in var.security_groups : group.identifier => group }

  name                     = each.value.sg_name
  description              = each.value.sg_description
  vpc_id                   = each.value.vpc_id
  ingress_with_cidr_blocks = try(each.value.ingress_with_cidr_blocks, [])
  egress_with_cidr_blocks  = try(each.value.egress_with_cidr_blocks, [])
  ingress_with_source_security_group_id = try(each.value.ingress_with_source_security_group_id, [])
  tags                     = each.value.sg_tags

  use_name_prefix = false

  create_timeout = var.create_timeout
  delete_timeout = var.delete_timeout
}
