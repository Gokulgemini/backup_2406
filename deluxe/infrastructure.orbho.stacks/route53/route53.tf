 module "aws-route53" {
  source = "git::ssh://git@bitbucket.org/deluxe-development/aws-route53.git//?ref=1.0.1"
  create_zone           = var.create_zone
  create_public_record  = var.create_public_record
  create_private_record = var.create_private_record
  zone_ids              = var.zone_ids
  records_jsonencoded   = var.records_jsonencoded
  tags                  = var.tags
 }