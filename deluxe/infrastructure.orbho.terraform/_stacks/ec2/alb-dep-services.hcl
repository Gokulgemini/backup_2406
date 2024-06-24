locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))
  account_vars     = read_terragrunt_config(find_in_parent_folders("account.hcl"))

  # Extract out common variables for reuse
  app_environment   = lower(local.environment_vars.locals.app_environment)
  infra_environment = lower(local.account_vars.locals.account_name)
  app_name          = "${local.infra_environment}-dep-alb-services"
  #certificate_arn   = local.account_vars.locals.certificate_arn

  source_base_url   = "git::ssh://git@bitbucket.org/deluxe-development/infrastructure.orboanywhereinfra.terraform.stacks.git//alb-stack"
}

dependency "common-tags" {
  config_path = "${get_terragrunt_dir()}/../overlay-common-tags"

  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy"]
  mock_outputs = {
    asg_common_tags = [
      {
        key                 = "AccountId"
        propagate_at_launch = true
        value               = "3432"
      }
    ]
    common_tags = {
      "AppName" = "${local.account_vars.locals.app_name}"
    }
  }
}

dependency "vpc-info" {
  config_path = "${get_terragrunt_dir()}/../../_global/vpc-info"

  mock_outputs_allowed_terraform_commands = ["validate", "plan", "destroy"]
  mock_outputs = {
    vpc_id              = "123"
    alb_subnets         = ["subnet-123", "subnet-456"]
    db_prim_subnet_ids  = ["subnet-123", "subnet-456", "subnet-789", "subnet-111"]
    app_subnets         = ["app-123", "app-456"]
    alb_security_groups = ["sg-1", "sg-2"]
    alb_sg_ids          = ["alb-1", "alb-2"]
    app_sg_id           = "app-sg-id-1"
    baseline_sg_id      = "baseline-sg-id-1"
    db_sg_id            = "db-sg-id-1"
    web_sg_id           = "web-sg-id-1"
    web_subnet_ids      = ["subnet-123", "subnet-456"]
  }
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  # Load Balancers & Target Groups
  alb_name           = "${local.app_name}"
  vpc_id             = dependency.vpc-info.outputs.vpc_id
  alb_subnets        = local.environment_vars.locals.vpc_info.alb_subnets
  idle_timeout       = 3000

  alb_security_groups = concat(
    [
      dependency.vpc-info.outputs.baseline_sg_id,
      dependency.vpc-info.outputs.web_sg_id
    ],
    dependency.vpc-info.outputs.alb_sg_ids
  )

   alb_target_groups = [{
    "name"             = "${local.app_environment}-orbo-services-https",
    "backend_protocol" = "HTTPS",
    "backend_port"     = 443,
    "target_type" = "instance"
    health_check = {
      enabled = true
      path    = "/"
      port                = 443
      healthy_threshold   = 5
      unhealthy_threshold = 5
      timeout             = 5
      interval            = 30
      protocol            = "HTTPS"
      matcher = "200"
    }
    },
    {
      "name"             = "${local.app_environment}-orbo-engine-https",
      "backend_protocol" = "HTTPS",
      "backend_port"     = 443,
      "target_type" = "instance"
      health_check = {
        enabled = true
        path    = "/"
        port                = 443
        healthy_threshold   = 5
        unhealthy_threshold = 5
        timeout             = 5
        interval            = 30
        protocol            = "HTTPS"
        matcher = "200"
      }
  }]

  http_tcp_listeners = [{
    port               = 80
    protocol           = "HTTP"
    target_group_index = 0
    }, {
    port               = 8443
    protocol           = "HTTP"
    target_group_index = 1
  }]

  # https_listeners = [
  #   {
  #     port               = 443
  #     protocol           = "HTTPS"
  #     certificate_arn    = local.certificate_arn
  #     target_group_index = 0
  #   }
  #   ]

  alb_tags = merge({ "TerraformPath" = path_relative_to_include() }, dependency.common-tags.outputs.common_tags)
}

