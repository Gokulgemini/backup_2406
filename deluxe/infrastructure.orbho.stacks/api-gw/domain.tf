resource "aws_api_gateway_domain_name" "main" {
  domain_name              = aws_acm_certificate.main.domain_name
  regional_certificate_arn = aws_acm_certificate.main.arn

  endpoint_configuration {
    types = ["REGIONAL"]
  }
}

resource "aws_api_gateway_base_path_mapping" "main" {
  api_id      = aws_api_gateway_rest_api.main.id
  domain_name = aws_api_gateway_domain_name.main.domain_name
  stage_name  = aws_api_gateway_stage.main.stage_name
}