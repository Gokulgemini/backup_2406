resource "aws_api_gateway_stage" "main" {
  depends_on = [aws_cloudwatch_log_group.rest_logs]
  deployment_id = aws_api_gateway_deployment.main.id
  rest_api_id   = aws_api_gateway_rest_api.main.id
  stage_name    = var.stage_name
}

resource "aws_api_gateway_method_settings" "example" {
  rest_api_id = aws_api_gateway_rest_api.main.id
  stage_name  = aws_api_gateway_stage.main.stage_name
  method_path = "*/*"

  settings {
    logging_level = "INFO"
    metrics_enabled = true
    data_trace_enabled = true
  }
}