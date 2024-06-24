locals {
  apigateway_vpce = "vpce-04db815ab36ba97b2"
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
    vpc_id              = "vpc-0241f6c165bbdc172"
    alb_subnets         = ["subnet-099210dc098aa0f6b", "subnet-06231754d3ae178cd"]
    db_prim_subnet_ids  = ["subnet-057699888fce14112", "subnet-0398e9b7202d2edb5"]
    app_subnets         = ["subnet-06529c3372f11b261", "subnet-0b98fa4e002c9fd91"]
    alb_security_groups = ["sg-037921d506f354660"]
    alb_sg_ids          = ["sg-037921d506f354660"]
    app_sg_id           = "sg-055e0a3cbe85013ba"
    baseline_sg_id      = "sg-0e569d92e262b0e1b"
    db_sg_id            = "sg-02c5cb6f8064ea0ed"
    web_sg_id           = "sg-0bf72a01b07b1bd1b"
    web_subnet_ids      = ["subnet-046cc16801c078d32", "subnet-00130e162c14877f2"]
  }
}
