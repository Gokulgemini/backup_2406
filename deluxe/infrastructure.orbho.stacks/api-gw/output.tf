output "curl_domain_url" {
  depends_on = [aws_api_gateway_base_path_mapping.main]

  description = "API Gateway Domain URL (self-signed certificate)"
  value       = "curl -H 'Host: ${var.rest_api_domain_name}' https://${aws_api_gateway_domain_name.main.regional_domain_name}${var.rest_api_path} # may take a minute to become available on initial deploy"
}

output "curl_stage_invoke_url" {
  description = "API Gateway Stage Invoke URL"
  value       = "curl ${aws_api_gateway_stage.main.invoke_url}${var.rest_api_path}"
}

output "vpc_endpoint_api_dns_name" {
  value = aws_vpc_endpoint.api-vpce[0].dns_entry[0]["dns_name"]
}

output "vpc_endpoint_log_dns_name" {
  value = aws_vpc_endpoint.cw-vpce[0].dns_entry[0]["dns_name"]
}

output "vpc_endpoint_lb_dns_name" {
  value = aws_vpc_endpoint.lb-vpce[0].dns_entry[0]["dns_name"]
}

output "cwlog_group_name" {
  value = aws_cloudwatch_log_group.rest_logs.arn
}