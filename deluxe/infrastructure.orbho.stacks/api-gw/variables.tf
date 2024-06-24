variable "aws_region" {
  default     = "us-east-1"
  description = "AWS Region to deploy example API Gateway REST API"
  type        = string
}

variable "rest_api_domain_name" {
  default     = "dep.xuat.tm.deluxe.com"
  description = "Domain name of the API Gateway REST API for self-signed TLS certificate"
  type        = string
}

variable "rest_api_name" {
  default     = "api-gateway-rest-api-dep-dev"
  description = "Name of the API Gateway REST API (can be used to trigger redeployments)"
  type        = string
}

variable "rest_api_path" {
  default     = "/path1"
  description = "Path to create in the API Gateway REST API (can be used to trigger redeployments)"
  type        = string
}

variable "stage_name" {
  default     = "dev"
  description = "Stage Name for Rest API"
  type        = string
}

variable "aws_apigw_vpce_sg_id" {
  default     = [""]
  description = "Security Group ID for APIGW Private VPCE"
  type        = list(string)
}

variable "aws_apigw_vpce_subnets" {
  default     = [""]
  type        = list(string)
  description = "Subnet ID used for APIGW VPCE"
}

variable "vpc_id" {
  default     = ""
  type        = string
  description = "VPC ID used for VPCE"
}

variable "apigw_cwlogs_role_arn" {
  default     = ""
  type        = string
  description = "Role ARN for Cloud Watch Logs to be used by API GW"
}

variable "endpoint_configuration" {
  default = "PRIVATE"
  type    = string
  description = "Endpoint Configuration for Rest API."
}

variable "target_group_ips" {
  default = [""]
  type    = list(string)
  description = "List of string for Target IPs"
}