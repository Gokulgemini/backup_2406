default['ec_nginx']['user'] = 'deploy'
default['ec_nginx']['behind_proxy'] = true
default['ec_nginx']['pool_size'] = 12
default['ec_nginx']['apps'] = [ {name: 'api_verifyvalid', server_names: ['api.verifyvalid.com']} ]
default['ec_nginx']['s3_artifact_bucket'] = "echecks-infra-default"
default['ec_nginx']['s3_artifact_folder'] = "/ssl-certs/"
