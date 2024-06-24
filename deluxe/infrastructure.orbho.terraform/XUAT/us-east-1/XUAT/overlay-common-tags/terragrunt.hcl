# Include all settings from the root terragrunt.hcl file
include "root" {
  path = find_in_parent_folders()
}

include "overlay-common-tags" {
  path = "${get_terragrunt_dir()}/../../../../_stacks/overlay-common-tags.hcl"
  expose = true
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "${include.overlay-common-tags.locals.source_base_url}?ref=1.0.1"
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
}