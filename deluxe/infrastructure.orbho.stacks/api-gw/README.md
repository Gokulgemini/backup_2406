<!-- BEGIN_TF_DOCS -->
## Requirements

No requirements.

## Providers

| Name | Version |
|------|---------|
| <a name="provider_aws"></a> [aws](#provider\_aws) | n/a |
| <a name="provider_tls"></a> [tls](#provider\_tls) | n/a |

## Modules

No modules.

## Resources

| Name | Type |
|------|------|
| [aws_acm_certificate.main](https://registry.terraform.io/providers/hashicorp/aws/latest/docs/resources/acm_certificate) | resource |
| [aws_api_gateway_base_path_mapping.main](https://registry.terraform.io/providers/hashicorp/aws/latest/docs/resources/api_gateway_base_path_mapping) | resource |
| [aws_api_gateway_deployment.main](https://registry.terraform.io/providers/hashicorp/aws/latest/docs/resources/api_gateway_deployment) | resource |
| [aws_api_gateway_domain_name.main](https://registry.terraform.io/providers/hashicorp/aws/latest/docs/resources/api_gateway_domain_name) | resource |
| [aws_api_gateway_method_settings.example](https://registry.terraform.io/providers/hashicorp/aws/latest/docs/resources/api_gateway_method_settings) | resource |
| [aws_api_gateway_rest_api.main](https://registry.terraform.io/providers/hashicorp/aws/latest/docs/resources/api_gateway_rest_api) | resource |
| [aws_api_gateway_stage.main](https://registry.terraform.io/providers/hashicorp/aws/latest/docs/resources/api_gateway_stage) | resource |
| [aws_cloudwatch_log_group.rest_logs](https://registry.terraform.io/providers/hashicorp/aws/latest/docs/resources/cloudwatch_log_group) | resource |
| [aws_vpc_endpoint.api-vpce](https://registry.terraform.io/providers/hashicorp/aws/latest/docs/resources/vpc_endpoint) | resource |
| [aws_vpc_endpoint.cw-vpce](https://registry.terraform.io/providers/hashicorp/aws/latest/docs/resources/vpc_endpoint) | resource |
| [aws_vpc_endpoint.lb-vpce](https://registry.terraform.io/providers/hashicorp/aws/latest/docs/resources/vpc_endpoint) | resource |
| [tls_private_key.main](https://registry.terraform.io/providers/hashicorp/tls/latest/docs/resources/private_key) | resource |
| [tls_self_signed_cert.main](https://registry.terraform.io/providers/hashicorp/tls/latest/docs/resources/self_signed_cert) | resource |
| [aws_region.current](https://registry.terraform.io/providers/hashicorp/aws/latest/docs/data-sources/region) | data source |

## Inputs

| Name | Description | Type | Default | Required |
|------|-------------|------|---------|:--------:|
| <a name="input_aws_apigw_vpce_sg_id"></a> [aws\_apigw\_vpce\_sg\_id](#input\_aws\_apigw\_vpce\_sg\_id) | Security Group ID for APIGW Private VPCE | `list(string)` | <pre>[<br>  ""<br>]</pre> | no |
| <a name="input_aws_apigw_vpce_subnets"></a> [aws\_apigw\_vpce\_subnets](#input\_aws\_apigw\_vpce\_subnets) | Subnet ID used for APIGW VPCE | `list(string)` | <pre>[<br>  ""<br>]</pre> | no |
| <a name="input_aws_region"></a> [aws\_region](#input\_aws\_region) | AWS Region to deploy example API Gateway REST API | `string` | `"us-east-1"` | no |
| <a name="input_rest_api_domain_name"></a> [rest\_api\_domain\_name](#input\_rest\_api\_domain\_name) | Domain name of the API Gateway REST API for self-signed TLS certificate | `string` | `"dep.xuat.tm.deluxe.com"` | no |
| <a name="input_rest_api_name"></a> [rest\_api\_name](#input\_rest\_api\_name) | Name of the API Gateway REST API (can be used to trigger redeployments) | `string` | `"api-gateway-rest-api-dep-dev"` | no |
| <a name="input_rest_api_path"></a> [rest\_api\_path](#input\_rest\_api\_path) | Path to create in the API Gateway REST API (can be used to trigger redeployments) | `string` | `"/path1"` | no |
| <a name="input_stage_name"></a> [stage\_name](#input\_stage\_name) | Stage Name for Rest API | `string` | `"dev"` | no |
| <a name="input_vpc_id"></a> [vpc\_id](#input\_vpc\_id) | VPC ID used for VPCE | `string` | `""` | no |

## Outputs

| Name | Description |
|------|-------------|
| <a name="output_curl_domain_url"></a> [curl\_domain\_url](#output\_curl\_domain\_url) | API Gateway Domain URL (self-signed certificate) |
| <a name="output_curl_stage_invoke_url"></a> [curl\_stage\_invoke\_url](#output\_curl\_stage\_invoke\_url) | API Gateway Stage Invoke URL |
| <a name="output_cwlog_group_name"></a> [cwlog\_group\_name](#output\_cwlog\_group\_name) | n/a |
| <a name="output_vpc_endpoint_api_dns_name"></a> [vpc\_endpoint\_api\_dns\_name](#output\_vpc\_endpoint\_api\_dns\_name) | n/a |
| <a name="output_vpc_endpoint_lb_dns_name"></a> [vpc\_endpoint\_lb\_dns\_name](#output\_vpc\_endpoint\_lb\_dns\_name) | n/a |
| <a name="output_vpc_endpoint_log_dns_name"></a> [vpc\_endpoint\_log\_dns\_name](#output\_vpc\_endpoint\_log\_dns\_name) | n/a |
<!-- END_TF_DOCS -->