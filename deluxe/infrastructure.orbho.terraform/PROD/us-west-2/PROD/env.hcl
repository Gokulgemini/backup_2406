locals {
  app_environment = "prod"
  infra_environment = "prod"
  environment = "prod"
  application = "prod"
  env_tags = {
    "Environment" = "Production"
    "Application" = "devops-orbo"
    "DeploymentTeam" = "DevOps"
  }
   vpc_info = {
    vpc_id              = "vpc-05c7d1d201ad36773"
    alb_subnets         = ["subnet-0cc3707ead4085d26", "subnet-0cf5129bd6bf5e9ec"]
    db_prim_subnet_ids  = ["subnet-04a4d6efdb25bf01f", "subnet-0ba4cbba8fe65fea8"]
    app_subnets         = ["subnet-078f7b4b925e8d46f", "subnet-0e6e60a2ff6ec5461"]
    alb_security_groups = ["sg-0ea6a5940a9c69b01"]
    alb_sg_ids          = ["sg-0ea6a5940a9c69b01"]
    app_sg_id           = "sg-05ce75802dbb01789"
    baseline_sg_id      = "sg-0c0e4a5c8350f360d"
    db_sg_id            = "sg-03cb6019928e75aef"
    web_sg_id           = "sg-07b81c8fb4b226120"
    web_subnet_ids      = ["subnet-0d23325c3cd8c1f95", "subnet-0fa7aad51928270ab"]
  }
}
