resource "tls_private_key" "main" {
  algorithm = "RSA"
}

resource "tls_self_signed_cert" "main" {
  allowed_uses = [
    "key_encipherment",
    "digital_signature",
    "server_auth",
  ]
  dns_names             = [var.rest_api_domain_name]
  private_key_pem       = tls_private_key.main.private_key_pem
  validity_period_hours = 12

  subject {
    common_name  = var.rest_api_domain_name
    organization = "Deluxe Corp"
  }
}

resource "aws_acm_certificate" "main" {
  certificate_body = tls_self_signed_cert.main.cert_pem
  private_key      = tls_private_key.main.private_key_pem
}