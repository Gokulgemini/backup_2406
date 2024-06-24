locals {
  # Automatically load environment-level variables
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))

  # Extract out common variables for reuse
  app_environment = lower(local.environment_vars.locals.app_environment)
  chef_environment = local.environment_vars.locals.chef_environment
  modules_version = local.environment_vars.locals.dpxn_modules_version
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "git::git@github.com:verifyvalid/infrastructure.dpx_fulfillment.terraform.modules.git//ec2-asg?ref=${local.modules_version}"

  extra_arguments "common_var" {
      commands = [
        "apply",
        "apply-all",
        "plan-all",
        "destroy-all",
        "plan",
        "import",
        "push",
        "refresh"
      ]

      arguments = [
        "-var-file=${get_terragrunt_dir()}/${path_relative_from_include()}/common.tfvars",
      ]
    }
}

dependency "iam-role-ec2-dpxn-cmp-mock" {
  config_path = "${get_terragrunt_dir()}/../iam-role-ec2-dpxn-cmp-mock"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    iam_policy_policy_arn = "arn::"
    iam_policy_policy_id = "12344"
    iam_instance_profile_arn = "arn::"
  }
}

dependency "common-tags" {
  config_path = "${get_terragrunt_dir()}/../overlay-common-tags"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    asg_common_tags = [
      {
        key = "AccountId"
        propagate_at_launch = true
        value = "3432"
      }
    ]
    common_tags = {
      "AppName" = "DPXN"
    }
  }
}

dependency "key-pair-app" {
  config_path = "${get_terragrunt_dir()}/../../_global/key-pair-app"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    key_pair_fingerprint = "23dfs24"
    key_pair_key_name = "key-pair"
    key_pair_key_pair_id = "1232dfdsdf"
  }
}

dependency "vpc-info" {
  config_path = "${get_terragrunt_dir()}/../../_global/vpc-info"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    vpc_id = "123"
    alb_subnets = ["subnet-123","subnet-456"]
    db_prim_subnet_ids = ["subnet-123","subnet-456","subnet-789","subnet-111"]
    app_subnets = ["app-123", "app-456"]
    alb_security_groups = ["sg-1", "sg-2"]
    alb_sg_ids = ["alb-1", "alb-2"]
    app_sg_id = "app-sg-id-1"
    baseline_sg_id = "baseline-sg-id-1"
    db_sg_id = "db-sg-id-1"
    web_sg_id = "web-sg-id-1"
    web_subnet_ids = ["subnet-123","subnet-456"]
  }
}

dependency "s3-infra-bucket" {
  config_path = "${get_terragrunt_dir()}/../../_global/s3-infra-bucket"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    s3_bucket_id = "key_id"
    s3_bucket_arn = "arn::"
    s3_bucket_domain_name = "dsf"
  }
}

dependency "sg-dpxn-cmp-mock" {
  config_path = "${get_terragrunt_dir()}/../sg-dpxn-cmp-mock"

  mock_outputs_allowed_terraform_commands = ["validate","plan","destroy"]
  mock_outputs = {
    sg_ids = {
      cmp-mock-sg = {
        security_group_id          = "sg-123"
        security_group_name        = "sg-abc"
      }
    }
  }
}

# Include all settings from the root terragrunt.hcl file
include {
  path = find_in_parent_folders()
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  instance_type = "t3a.medium"
  iam_instance_profile = dependency.iam-role-ec2-dpxn-cmp-mock.outputs.iam_instance_profile_arn
  instance_key_name = dependency.key-pair-app.outputs.key_pair_key_name
  min_size = 1
  max_size = 1
  default_cooldown = 300
  desired_capacity = 1
  app_name = "cmp-mock"
  run_list = "role[dpxn_cmp_mock]"
  app_environment = local.app_environment
  chef_environment = local.chef_environment
  infra_bucket = dependency.s3-infra-bucket.outputs.s3_bucket_id
  security_groups = [ dependency.vpc-info.outputs.baseline_sg_id, dependency.vpc-info.outputs.app_sg_id, dependency.sg-dpxn-cmp-mock.outputs.sg_ids["cmp-mock"].security_group_id ]
  vpc_zone_identifier = dependency.vpc-info.outputs.app_subnets
  asg_tags = flatten([ dependency.common-tags.outputs.asg_common_tags, [
  {
    "key" = "TerraformPath"
    "value" = path_relative_to_include()
    "propagate_at_launch" = true
  },{
    "key" = "ServerRole"
    "value" = "cmp_mock"
    "propagate_at_launch" = true
  }]])
}
