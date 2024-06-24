locals {
    key_name  = var.create_keypair ? module.key_pair.key_pair_key_name : var.instance_key_name
    ami_id    = var.ami_id
}