# Include all settings from the root terragrunt.hcl file
include "root" {
  path = find_in_parent_folders()
}

# Pull in stack you want.
include "security-groups" {
  path = "${get_terragrunt_dir()}/../../../../_stacks/engine-security-groups.hcl"
  expose = true
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "${include.security-groups.locals.source_base_url}?ref=security-groups"
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
}
