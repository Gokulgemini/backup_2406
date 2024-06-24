resource "aws_vpc_endpoint" "api-vpce" {
  count = 1

  private_dns_enabled = false
  security_group_ids  = var.aws_apigw_vpce_sg_id
  service_name        = "com.amazonaws.${data.aws_region.current.name}.execute-api"
  subnet_ids          = var.aws_apigw_vpce_subnets
  vpc_endpoint_type   = "Interface"
  vpc_id              = var.vpc_id
}

resource "aws_vpc_endpoint" "cw-vpce" {
  count = 1

  private_dns_enabled = false
  security_group_ids  = var.aws_apigw_vpce_sg_id
  service_name        = "com.amazonaws.${data.aws_region.current.name}.logs"
  subnet_ids          = var.aws_apigw_vpce_subnets
  vpc_endpoint_type   = "Interface"
  vpc_id              = var.vpc_id
}

resource "aws_vpc_endpoint" "lb-vpce" {
  count = 1

  private_dns_enabled = false
  security_group_ids  = var.aws_apigw_vpce_sg_id
  service_name        = "com.amazonaws.${data.aws_region.current.name}.elasticloadbalancing"
  subnet_ids          = var.aws_apigw_vpce_subnets
  vpc_endpoint_type   = "Interface"
  vpc_id              = var.vpc_id
}

data "aws_region" "current" {}