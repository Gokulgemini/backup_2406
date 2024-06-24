# env_classification = node['dna']['notengineyard']['environment']['framework_env'] == 'production' ? 'production' : 'staging'
# default['custom-monit'] ||= {}
#
# # Some per app/env config
# echecks_app_name = env_classification == 'production' ? 'VerifyValid' : 'verifyvalid'
# default['custom-monit']['echecks_path'] = '/data/' + echecks_app_name + '/current'
#
# # The next two lines have to match with the application
# default['custom-monit']['echecks_delayed_job_worker_count'] = 7 # currently two queues 5 + 2
# default['custom-monit']['echecks_delayed_job_options'] = '--pool=batch:2 --pool=*:5'
#
# registrar_app_name = env_classification == 'production' ? 'vv_registrar' : 'Registrar'
# default['custom-monit']['registrar_path'] = '/data/' + echecks_app_name + '/current'



default['ec_monit_config']['unicorn']['apps'] = [{ "name": "default" }]

default['ec_monit_config']['delayed_job']['framework_env'] = 'preproduction'
default['ec_monit_config']['delayed_job']['echecks_path'] = '/data/verifyvalid/current'
default['ec_monit_config']['delayed_job']['delayed_job_start'] = '/bin/delayed_job'
default['ec_monit_config']['delayed_job']['app_name'] = 'verifyvalid'
default['ec_monit_config']['delayed_job']['worker_count'] = 13 # currently four queues 5 + 2 + 1 + 5
# default['ec_monit_config']['delayed_job']['job_options'] = '--pool=batch:2 --pool=user_realtime_email:1 --pool=*:5'
default['ec_monit_config']['delayed_job']['job_options'] = '--pool=batch:2 --pool=variable_print:1 --pool=api_batch_check_delivery,email_retrieval_notifier:5 --pool=check_delivery,email,check_deliveries,hook_event,cmp,bulk_operation,yoodle,rep_code_imports,custom_billing,yodlee,mulesoft,default:5'
# default['ec_monit_config']['delayed_job']['registrar_app_name'] = 'Registrar'
# default['ec_monit_config']['delayed_job']['registrar_path'] = '/data/Registrar/current'
# default['ec_monit_config']['delayed_job']['onboard_path'] = '/data/vv_onboard/current/'
#
# env_classification = node['dna']['notengineyard']['environment']['framework_env'] == 'production' ? 'production' : 'staging'
# default['custom-monit'] ||= {}
#
# # Some per app/env config
# echecks_app_name = env_classification == 'production' ? 'VerifyValid' : 'verifyvalid'
# default['custom-monit']['echecks_path'] = '/data/' + echecks_app_name + '/current'
#
# # The next two lines have to match with the application
# default['custom-monit']['echecks_delayed_job_worker_count'] = 7 # currently two queues 5 + 2
# default['custom-monit']['echecks_delayed_job_options'] = '--pool=batch:2 --pool=*:5'
#
# registrar_app_name = env_classification == 'production' ? 'vv_registrar' : 'Registrar'
# default['custom-monit']['registrar_path'] = '/data/' + echecks_app_name + '/current'
default['ec_monit_config']['clamav']['log_file'] = '/var/log/clamav/clamd.log'
default['ec_monit_config']['clamav']['pid_file'] = '/var/run/clamav/daemon-clamd.pid'
default['ec_monit_config']['clamav']['pid_dir'] = '/var/run/clamav'

default['ec_monit_config']['resque']['worker_count'] = 5
