data "template_file" "domain_join" {
  template = file("${path.cwd}/templates/deluxe-domain-join.ps1")
  vars = {
    directory_id    = var.directory_id
    windows_version = var.windows_version
  }
}

data "template_cloudinit_config" "domain_join_config" {
  part {
    content_type = "text/x-shellscript"
    content      = data.template_file.domain_join.rendered
  }
}