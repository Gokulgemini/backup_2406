output "rds_role_arn" {
  value = aws_iam_role.rds_role.arn
}

output "rds_policy_arn" {
  value = aws_iam_policy.rds_policy.arn
}

output "s3_role_arn" {
  value = aws_iam_role.s3_role.arn
}

output "s3_policy_arn" {
  value = aws_iam_policy.s3_policy.arn
}

output "ec2_role_arn" {
  value = aws_iam_role.ec2_role.arn
}

output "ec2_policy_arn" {
  value = aws_iam_policy.ec2_policy.arn
}
