name 'ec_rails_keys'
maintainer 'Deluxe eChecks'
maintainer_email 'no-reply@deluxe.com'
description 'Downloads rails keys from s3'
long_description 'Downloads rails keys from s3'
version '1.1.0'
chef_version '>= 12.1' if respond_to?(:chef_version)
supports 'ubuntu', '>= 18.04'

depends 's3_file', '~> 2.8.1'
depends 'ec_app_config'
