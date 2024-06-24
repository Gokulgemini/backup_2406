locals {
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))
  account_vars     = read_terragrunt_config(find_in_parent_folders("account.hcl"))
  infra_environment = lower(replace(local.environment_vars.locals.infra_environment, "_", "-"))
  region_vars       = read_terragrunt_config(find_in_parent_folders("region.hcl"))
  source_base_url = "git::ssh://git@bitbucket.org/deluxe-development/infrastructure.orboanywhereinfra.terraform.stacks.git//api-gw"
  app_env = local.environment_vars.locals.app_environment
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  aws_region = local.region_vars.locals.aws_region
  private_dns_enabled = true
  stage_name = local.environment_vars.locals.app_environment
  rest_api_domain_name = "orbo-recognition-service.orbo.dep.${local.app_env}.tm.deluxe.com"
  rest_api_name = "dep-api-gateway-${local.app_env}"
  apigw_cwlogs_role_arn ="arn:aws:iam::${local.account_vars.locals.aws_account_id}:role/dep-${local.app_env}-rest-api-orbo-recognition"
  rest_api_path = "/${local.app_env}"
  body = templatefile("swagger/documentextractionservice.tftpl", {
    aws_region = local.region_vars.locals.aws_region
    server_url = "orbo-recognition-service.orbo.dep.${local.app_env}.tm.deluxe.com",
    api_integration_credentials = "arn:aws:iam::${local.account_vars.locals.aws_account_id}:role/dep-${local.app_env}-rest-api-orbo-recognition"
    api_integration_uri = "arn:aws:apigateway:${local.region_vars.locals.aws_region}:sqs:path/${local.account_vars.locals.aws_account_id}/${local.app_env}-dep-batch-queue"
    vpc_endpoint_id = local.environment_vars.locals.apigateway_vpce
  })
}