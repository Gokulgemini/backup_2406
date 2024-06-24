 # Include all settings from the root terragrunt.hcl file
include "root" {
  path = find_in_parent_folders()
}

# Pull in stack you want.
include "sqs-dep-batch-queue" {
  path   = "${get_terragrunt_dir()}/../../../../_stacks/sqs/sqs-dep-batch-queue.hcl"
  expose = true
}

# Terragrunt will copy the Terraform configurations specified by the source parameter, along with any files in the
# working directory, into a temporary folder, and execute your Terraform commands in that folder.
terraform {
  source = "${include.sqs-dep-batch-queue.locals.source_base_url}?ref=main"
}

# These are the variables we have to pass in to use the module specified in the terragrunt configuration above
inputs = {
}