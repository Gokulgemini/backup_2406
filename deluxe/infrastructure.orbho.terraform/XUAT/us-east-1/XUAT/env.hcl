locals {
  app_environment = "xuat"
  infra_environment = "xuat"
  environment = "xuat"
  application = "dep"
  env_tags = {
    "Environment" = "XUAT"
    "Application" = "devops-orbo"
    "DeploymentTeam" = "DevOps"
  }
   vpc_info = {
    vpc_id              = "vpc-041a1dd0a0a72db5f"
    alb_subnets         = ["subnet-0b7f1de698b38933a", "subnet-072c132b61b577a62"]
    db_prim_subnet_ids  = ["subnet-01a367d2432f2905d", "subnet-02f2c2e8d7e64ac4c"]
    app_subnets         = ["subnet-07cebf6fdfb595845", "subnet-093abf553a1934c5d"]
    alb_security_groups = ["sg-0a139b231125239c0"]
    alb_sg_ids          = ["sg-0a139b231125239c0"]
    app_sg_id           = "sg-0b08ac2154d79097e"
    baseline_sg_id      = "sg-0b56a31669a267759"
    db_sg_id            = "sg-0cbdb4d54c92f45da"
    web_sg_id           = "sg-0e354c1c77d1e82bb"
    web_subnet_ids      = ["subnet-0875ae9b2c0ff195b", "subnet-0024495c5987a4a46"]
  }
}
