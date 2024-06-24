resource "aws_api_gateway_account" "main" {
  cloudwatch_role_arn = var.apigw_cwlogs_role_arn
}

resource "aws_cloudwatch_log_group" "rest_logs" {
  name              = "API-Gateway-Execution-Logs_${aws_api_gateway_rest_api.main.id}/${var.stage_name}"
  retention_in_days = 7
}