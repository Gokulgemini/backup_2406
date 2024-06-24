# Include all settings from the root terragrunt.hcl file
include "root" {
  path = find_in_parent_folders()
}

include "key-pair-app" {
  path = "${get_terragrunt_dir()}/../../../../_stacks/key-pair-app.hcl"
  expose = true
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "${include.key-pair-app.locals.source_base_url}?ref=feature/s3-upload-keys"
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
}