locals {
  app_environment = "dev"
  infra_environment = "dev"
  environment = "dev"
  application = "dep"
  env_tags = {
    "Environment" = "Development"
    "Application" = "devops-orbo"
    "DeploymentTeam" = "DevOps"
  }
  vpc_info = {
    vpc_id              = "vpc-092680ef1316e4316"
    alb_subnets         = ["subnet-0355b6868faeb1b28", "subnet-0c6c2cdf7897b9f40"]
    db_prim_subnet_ids  = ["subnet-09ea59e472c64445d", "subnet-023c078656651746c"]
    app_subnets         = ["subnet-0329775205c6346ed", "subnet-0de03946e680830e6"]
    alb_security_groups = ["sg-0456b6f440ada4362"]
    alb_sg_ids          = ["sg-0456b6f440ada4362"]
    app_sg_id           = "sg-03738d7f6dd7c446a"
    baseline_sg_id      = "sg-06a9070ca9466663e"
    db_sg_id            = "sg-0976ea3f02604c408"
    web_sg_id           = "sg-075a51bf64c3be33a"
    web_subnet_ids      = ["subnet-0bf1f3faa47fbb0dc", "subnet-0db7b21fbf949bdf9"]
  }
}
