 module "iam_policy" {
    source = "git::ssh://git@bitbucket.org/deluxe-development/aws-iam-role-with-policy.git//?ref=1.4.0"
    create_role        = var.create_role
    create_policy      = var.create_policy
    create_instance_profile = var.create_instance_profile
    role_name          = var.role_name
    role_description   = var.role_description
    policy_name        = var.policy_name
    policy_description = var.policy_description
    role_path          = var.role_path
    attach_managed_policy = var.attach_managed_policy
    managed_policy_arns = var.managed_policy_arns
    policy_statement   = var.policy_statement
    trusted_entities = var.trusted_entities
    tags    = var.tags
 }