output "iam_instance_profile_id" {
  value =  module.iam_policy.iam_instance_profile_id
}

output "iam_instance_profile_arn" {
  value =  module.iam_policy.iam_instance_profile_arn
}

output "iam_role_arn" {
  value = module.iam_policy.iam_role_arn
}

output "iam_role_name" {
  value = module.iam_policy.iam_role_name
}

output "iam_role_path" {
  value = module.iam_policy.iam_role_path
}

output "iam_role_unique_id" {
  value = module.iam_policy.iam_role_unique_id
}

output "role_requires_mfa" {
  value =  module.iam_policy.role_requires_mfa
}