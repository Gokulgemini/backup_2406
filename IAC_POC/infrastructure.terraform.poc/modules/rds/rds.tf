module "db_instance" {
  source = "terraform-aws-modules/rds/aws"

  identifier             = var.identifier
  engine                 = var.engine
  engine_version         = var.engine_version
  instance_class         = var.instance_class
  allocated_storage      = var.allocated_storage
  storage_type           = var.storage_type
  iops                   = var.iops
  storage_encrypted      = var.storage_encrypted
  kms_key_id             = var.kms_key_id
  license_model          = var.license_model  
  db_name                = var.db_name
  username               = var.username
  password               = var.password
  port                   = var.port
  vpc_security_group_ids = var.vpc_security_group_ids
  #subnet_ids            = var.subnet_ids
  apply_immediately      = var.apply_immediately
  maintenance_window     = var.maintenance_window
  deletion_protection    = var.deletion_protection
  create_db_option_group = var.create_db_option_group
  create_db_parameter_group = var.create_db_parameter_group  
}