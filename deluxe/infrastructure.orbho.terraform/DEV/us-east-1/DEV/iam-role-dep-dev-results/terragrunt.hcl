# Include all settings from the root terragrunt.hcl file
include "root" {
  path = find_in_parent_folders()
}

include "iam-role-dep-results" {
  path = "${get_terragrunt_dir()}/../../../../_stacks/iam/iam-role-dep-results/iam-role-dep-results.hcl"
  expose = true
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "${include.iam-role-dep-results.locals.source_base_url}?ref=1.4.0"
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
}