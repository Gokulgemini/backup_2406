resource "aws_instance" "this" {
  count = var.create_ec2 ? 1 : 0

  ami                         = local.ec2_ami_id
  instance_type               = var.instance_type
  user_data_base64            = local.user_data_base64
  user_data_replace_on_change = var.user_data_replace_on_change
  subnet_id                   = var.subnet_id
  vpc_security_group_ids      = var.instance_security_groups
  key_name                    = local.key_name
  monitoring                  = var.monitoring
  iam_instance_profile        = var.iam_instance_profile
  associate_public_ip_address = var.associate_public_ip_address
  ebs_optimized               = var.ebs_optimized

  dynamic "root_block_device" {
    for_each = var.root_block_device
    content {
      delete_on_termination = lookup(root_block_device.value, "delete_on_termination", null)
      encrypted             = lookup(root_block_device.value, "encrypted", null)
      iops                  = lookup(root_block_device.value, "iops", null)
      kms_key_id            = lookup(root_block_device.value, "kms_key_id", null)
      volume_size           = lookup(root_block_device.value, "volume_size", null)
      volume_type           = lookup(root_block_device.value, "volume_type", null)
      throughput            = lookup(root_block_device.value, "throughput", null)
    }
  }

  dynamic "ebs_block_device" {
    for_each = var.ebs_block_device
    content {
      delete_on_termination = lookup(ebs_block_device.value, "delete_on_termination", null)
      device_name           = ebs_block_device.value.device_name
      encrypted             = lookup(ebs_block_device.value, "encrypted", null)
      iops                  = lookup(ebs_block_device.value, "iops", null)
      kms_key_id            = lookup(ebs_block_device.value, "kms_key_id", null)
      snapshot_id           = lookup(ebs_block_device.value, "snapshot_id", null)
      volume_size           = lookup(ebs_block_device.value, "volume_size", null)
      volume_type           = lookup(ebs_block_device.value, "volume_type", null)
      throughput            = lookup(ebs_block_device.value, "throughput", null)
    }
  }

  timeouts {
    create = lookup(var.timeouts, "create", null)
    update = lookup(var.timeouts, "update", null)
    delete = lookup(var.timeouts, "delete", null)
  }

  tags        = merge({ "Name" = var.instance_name }, var.instance_tags)
  volume_tags = var.enable_volume_tags ? merge({ "Name" = var.instance_name }, var.volume_tags) : null

  # Always ignore tag changes, Since all servers need to be in CyberArk and that is controlled by tags after server creation
  # we don't want a server recreated because the tags have been updated.
  lifecycle {
    ignore_changes = [ tags ]
  }

}
