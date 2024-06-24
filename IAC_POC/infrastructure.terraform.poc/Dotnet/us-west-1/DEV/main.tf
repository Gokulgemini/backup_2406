module"vpc"{
    source = "../../../global_modules/vpc/"

    environment                        = "dev"
    name                               = "vpc_java" # Overridning the name defined in variable file
    cidr                               = "10.0.0.0/16"
    azs                                = ["us-west-1b", "us-west-1c"]
    public_subnets                     = ["10.0.101.0/24", "10.0.102.0/24"]
    private_subnets                    = ["10.0.1.0/24", "10.0.2.0/24"]
    database_subnets                   = ["10.0.151.0/24", "10.0.152.0/24"]
    create_database_subnet_group       = true
    create_database_subnet_route_table = true
    enable_nat_gateway                 = false
    single_nat_gateway                 = false
}

module"sg"{
    source = "../../../global_modules/security_group/"

    sg_name            = "sg_java"
    vpc_id             = module.vpc.vpc_id
    ingress_cidr_block = ["10.0.0.0/16"]
    ingress_rule       = ["https-443-tcp"]
    from_port          = 8080
    to_port            = 8090
    protocal           = "tcp"
    cidr_block         = "0.0.0.0/16"
}

module"iam"{
    source = "../../../modules/iam/"

    rds_role_name        = "rds-role"
    rds_policy_name      = "rds-policy"
    s3_role_name         = "s3-role"
    s3_policy_name       = "s3-policy"
    ec2_role_name        = "ec2-role"
    ec2_policy_name      = "ec2-policy"
}

resource "aws_iam_role" "ec2_role" {
  name = "ec2_role"
  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [{
      Action = "sts:AssumeRole"
      Effect = "Allow"
      Principal = {
        Service = "ec2.amazonaws.com"
      }
    }]
  })
}

resource "aws_iam_instance_profile" "ec2_instance_profile" {
  name = "ec2_instance_profile"
  role = aws_iam_role.ec2_role.name
}

module"ec2"{
    source = "../../../modules/ec2/"

    instance_name          = "java-app"
    ami_id                 = "ami-08012c0a9ee8e21c4"
    iam_instance_profile   = aws_iam_instance_profile.ec2_instance_profile.name
    create_keypair         = true
    instance_type          = "t2.micro"
    instance_key_name      = "ec2-key"
    monitoring             = false
    vpc_security_group_ids = module.sg.security_group_ids
    subnet_id              = module.vpc.subnet_id
    region                 = "us-west-1"
}

