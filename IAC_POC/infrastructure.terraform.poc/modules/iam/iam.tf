resource "aws_iam_role" "rds_role" {
  name               = var.rds_role_name
  assume_role_policy = data.aws_iam_policy_document.rds_assume_role_policy.json
}

resource "aws_iam_policy" "rds_policy" {
  name        = var.rds_policy_name
  description = "IAM policy for RDS"
  policy      = data.aws_iam_policy_document.rds_policy.json
}

resource "aws_iam_role_policy_attachment" "rds_attachment" {
  role       = aws_iam_role.rds_role.name
  policy_arn = aws_iam_policy.rds_policy.arn
}

resource "aws_iam_role" "s3_role" {
  name               = var.s3_role_name
  assume_role_policy = data.aws_iam_policy_document.s3_assume_role_policy.json
}

resource "aws_iam_policy" "s3_policy" {
  name        = var.s3_policy_name
  description = "IAM policy for S3"
  policy      = data.aws_iam_policy_document.s3_policy.json
}

resource "aws_iam_role_policy_attachment" "s3_attachment" {
  role       = aws_iam_role.s3_role.name
  policy_arn = aws_iam_policy.s3_policy.arn
}

resource "aws_iam_role" "ec2_role" {
  name               = var.ec2_role_name
  assume_role_policy = data.aws_iam_policy_document.ec2_assume_role_policy.json
}

resource "aws_iam_policy" "ec2_policy" {
  name        = var.ec2_policy_name
  description = "IAM policy for EC2"
  policy      = data.aws_iam_policy_document.ec2_policy.json
}

resource "aws_iam_role_policy_attachment" "ec2_attachment" {
  role       = aws_iam_role.ec2_role.name
  policy_arn = aws_iam_policy.ec2_policy.arn
}

data "aws_iam_policy_document" "rds_assume_role_policy" {
  statement {
    actions = ["sts:AssumeRole"]
    principals {
      type        = "Service"
      identifiers = ["rds.amazonaws.com"]
    }
  }
}

data "aws_iam_policy_document" "rds_policy" {
  statement {
    actions   = ["rds:*"]
    resources = [
      "arn:aws:s3:::${var.rds_policy_name}/*"
    ]
  }
}

data "aws_iam_policy_document" "s3_assume_role_policy" {
  statement {
    actions = ["sts:AssumeRole"]
    principals {
      type        = "Service"
      identifiers = ["s3.amazonaws.com"]
    }
  }
}

data "aws_iam_policy_document" "s3_policy" {
  statement {
    actions = [
      "s3:PutObject",
      "s3:GetObject"
    ]
    resources = [
      "arn:aws:s3:::${var.s3_policy_name}/*"
    ]
  }
}

data "aws_iam_policy_document" "ec2_assume_role_policy" {
  statement {
    actions = ["sts:AssumeRole"]
    principals {
      type        = "Service"
      identifiers = ["ec2.amazonaws.com"]
    }
  }
}

data "aws_iam_policy_document" "ec2_policy" {
  statement {
    actions   = ["ec2:*"]
    resources = [
      "arn:aws:s3:::${var.ec2_policy_name}/*"
    ]
  }
}
