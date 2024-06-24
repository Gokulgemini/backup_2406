resource "aws_api_gateway_rest_api" "main" {
  body = file("${path.module}/swagger.json")

  name              = var.rest_api_name
  put_rest_api_mode = "merge"

  endpoint_configuration {
    types            = [var.endpoint_configuration]
    vpc_endpoint_ids = [aws_vpc_endpoint.api-vpce[0].id]
  }
}

resource "aws_api_gateway_deployment" "main" {
  rest_api_id = aws_api_gateway_rest_api.main.id

  triggers = {
    redeployment = sha1(aws_api_gateway_rest_api.main.body)
  }

  lifecycle {
    create_before_destroy = true
  }
}