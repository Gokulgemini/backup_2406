# Include all settings from the root terragrunt.hcl file
include "root" {
  path = find_in_parent_folders()
}

include "s3-infra-bucket" {
  path = "${get_terragrunt_dir()}/../../../../_stacks/s3-infra-bucket.hcl"
  expose = true
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "${include.s3-infra-bucket.locals.source_base_url}?ref=2.1.5"
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
}
