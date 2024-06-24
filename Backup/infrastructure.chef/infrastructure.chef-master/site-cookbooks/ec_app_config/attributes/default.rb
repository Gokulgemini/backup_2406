#default['ec_app_config']['app_name'] = 'default'
#default['ec_app_config']['environment'] = 'default'
default['ec_app_config']['default']['s3_artifact_bucket'] = 'default'
default['ec_app_config']['default']['s3_artifact_folder'] = 'default'

#default['ec_app_config']['templates'] = 'default'
default['ec_app_config']['environment'] = 'default_rails_env'

default['ec_app_config']['apps'] = ['verifyvalid', 'console', 'monitoring', 'registrar','onboard','donations','retrievals']

# default['ec_app_config']['app']['default']['config_files'] = [
#   'aws.yml': { 'use_s3': true },
#   'database.yml': { 'use_s3': false }
# ]

default['ec_app_config']['app'] ={
  'name': 'default',
  "configurations": {
    'aws_region': 'us-east-1',
    'access_key_id': 'foo',
    'secret_access_key': 'bar',
    's3_bucket_name': 'default',
    'api_host': 'default',
    'api_key': 'default',
    'app_name': 'default',
    'database': 'default',
    'db_adapter': 'default',
    'db_host': 'localhost',
    'db_reconnect': 'false',
    'db_username': 'deploy',
    'db_password': 'password',
    'db_encoding': 'utf8',
    'role_arn': "arn:aws:iam::800092304363:role/Deluxe-eChecks-EC2-Server-Role",
    'role_session_name': "verifyvalid-app-server"
  }
}




# default['ec_app_config']['aws_region'] = 'us-east-1'
# default['ec_app_config']['access_key_id'] = 'foo'
# default['ec_app_config']['secret_access_key'] = 'bar'
# default['ec_app_config']['s3_bucket_name'] = 'default'
# default['ec_app_config']['api_host'] = 'default'
# default['ec_app_config']['api_key'] = 'default'
# default['ec_app_config']['app_name'] = 'default'
# default['ec_app_config']['database'] = 'default'
# default['ec_app_config']['db_adapter'] = 'default'
# default['ec_app_config']['db_host'] = 'localhost'
# default['ec_app_config']['db_reconnect'] = 'false'
# default['ec_app_config']['db_username'] = 'deploy'
# default['ec_app_config']['db_password'] = 'password'
# default['ec_app_config']['db_encoding'] = 'utf8'
# default['ec_app_config']['role_arn'] = "arn:aws:iam::800092304363:role/Deluxe-eChecks-EC2-Server-Role"
# default['ec_app_config']['role_session_name'] = "verifyvalid-app-server"
