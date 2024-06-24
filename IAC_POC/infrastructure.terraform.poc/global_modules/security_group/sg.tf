module "security_group" {
  source = "terraform-aws-modules/security-group/aws"

  name        = var.sg_name
  description = "Security group for user-service with custom ports open within VPC, and PostgreSQL publicly open"
  vpc_id      = var.vpc_id

  ingress_cidr_blocks      = var.ingress_cidr_block
  ingress_rules            = var.ingress_rule
  ingress_with_cidr_blocks = [
    {
      from_port   = var.from_port
      to_port     = var.to_port
      protocol    = var.protocal
      description = "User-service ports"
      cidr_blocks = var.cidr_block
    },
  ]
}