
default['ec_rails_keys']['owner'] = "deploy"
default['ec_rails_keys']['group'] = "deploy"

#default['ec_rails_keys']['s3_artifact_folder'] = "/fake/keys/"
#default['ec_rails_keys']['s3_artifact_file'] = "encryption_file"
default['ec_rails_keys']['s3_artifact_bucket'] = "echecks-infra-default"

default['ec_rails_keys']['s3_files'] = [
  # "verify_valid_#{account_level_env}.iv",
  # "verify_valid_#{account_level_env}.key",
  "verify_valid_staging.iv",
  "verify_valid_staging.key",
]
