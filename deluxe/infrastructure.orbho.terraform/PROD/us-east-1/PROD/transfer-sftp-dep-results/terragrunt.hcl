# Include all settings from the root terragrunt.hcl file
include "root" {
  path = find_in_parent_folders()
}

include "transfer-sftp-dep-results" {
  path = "${get_terragrunt_dir()}/../../../../_stacks/sftp/transfer-sftp-dep-results.hcl"
  expose = true
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "${include.transfer-sftp-dep-results.locals.source_base_url}?ref=feature/sg-id-argument-bug-fix"
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
}