# Include all settings from the root terragrunt.hcl file
include "root" {
  path = find_in_parent_folders()
}

include "kms-dep-processors" {
  path = "${get_terragrunt_dir()}/../../../../_stacks/kms/kms-dep-processors.hcl"
  expose = true
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "${include.kms-dep-processors.locals.source_base_url}?ref=3.4.0"
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
}