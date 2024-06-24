module "acm" {
  source = "git::ssh://git@bitbucket.org:/deluxe-development/aws-acm.git//?ref=1.0.1"

  environment      = var.environment
  cert_bucket_name = var.cert_bucket_name
  tags             = var.acm_tags
  pem_password     = var.pem_password
  pfx_object       = var.pfx_object
  pfx_password     = var.pfx_password
}