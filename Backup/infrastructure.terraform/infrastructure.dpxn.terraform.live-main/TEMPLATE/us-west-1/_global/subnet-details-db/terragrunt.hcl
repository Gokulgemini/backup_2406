locals {
   modules_version = "1.1.0"
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "git::ssh://git@bitbucket.deluxe.com:7999/dtf/aws-subnet-details.git//?ref=${local.modules_version}"
}

dependency "vpc-info" {
  config_path = "${get_terragrunt_dir()}/../vpc-info"
    mock_outputs = {
    db_prim_subnet_ids = ["subnet-1122"]
    db_sec_subnet_ids = ["subnet-1133"]
  }
}

# Include all settings from the root terragrunt.hcl file
include {
  path = find_in_parent_folders()
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
  subnet_ids = concat(dependency.vpc-info.outputs.db_prim_subnet_ids, dependency.vpc-info.outputs.db_sec_subnet_ids)
}
