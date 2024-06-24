name 'production'
description 'The production chef environment infrastucture deployment of the production rails environment to the app environment infrastucture named production within the prod vpc'

cookbook_versions(
  'aws' => '~> 8.1.1',
  's3_file' => '~> 2.8.5',
  'ec_app_config' => '~> 1.1.23',
  'ec_monit' =>  '~> 1.1.0',
  'ec_monit_config' =>  '~> 1.2.1',
  'ec_pdftk' => '~> 1.0.1',
  'ec_clamav' =>  '~> 1.0.27',
  'ec_cloudwatch' =>  '~> 1.1.2',
  'ec_cron' =>  '~> 1.3.4',
  'ec_security_updates' =>  '~> 1.0.41',
  'ec_custom_fonts' =>  '~> 1.0.5',
  'packagecloud' =>  '~> 1.0.1',
  'yum-epel' =>  '~> 3.3.0',
  'runit' =>  '~> 5.1.2',
  'memcached' =>  '~> 5.1.1',
  'ec_memcached' =>  '~> 0.1.3',
  'mysql' =>  '~> 8.6.0',
  'ec_mysql' =>  '~> 0.1.8',
  'ec_netowl' =>  '~> 1.0.9',
  'ec_newrelic_infra' =>  '~> 1.0.7',
  'ohai' =>  '~> 5.3.0',
  'nginx' =>  '~> 10.0.2',
  'ssl_certificate' =>  '~> 2.1.0',
  'ec_nginx' =>  '~> 1.4.6',
  'ec_prompt' =>  '~> 1.2.1',
  'ec_common' => '~> 0.1.0',
  'ec_rails_keys' =>  '~> 1.1.0',
  'ec_ruby' =>  '~> 1.0.21',
  'line' => '~> 2.8.1',
  'brightbox-ruby' => '~> 1.2.2',
  'ec_unicorn' =>  '~> 1.0.23',
  'users' =>  '~> 5.4.0',
  'ec_users' =>  '~> 1.0.10',
  'dynatrace_oneagent' => '~> 1.0.0',
  'ec_deluxe_sudoers' => '~> 0.1.2'
)

override_attributes "vpc_env": "prod",
                    "rails_env": "production",
                    "domain": "echecks.com",
                    "ec_monit_config": {
                      "delayed_job": {
                        "worker_count": 15,
                        "job_options": "--pool=batch:4 --pool=variable_print:1 --pool=api_batch_check_delivery,email_retrieval_notifier:5 --pool=check_delivery,email,check_deliveries,hook_event,cmp,bulk_operation,yoodle,rep_code_imports,custom_billing,yodlee,mulesoft,default:5"
                      }
                    },
                    "dynatrace_oneagent": {
                      "host_group": "echecks_app_production"
                    },
                    "ec_ruby": {
                      "version": "3.1.2"
                    },
                    "clamav": {
                      "version": "0.103.9+dfsg-0ubuntu0.22.04.1"
                    },
                    "nodejs": {
                      "version": '20.9.0',
                      "binary": {
                        "checksum": "f0919f092fbf74544438907fa083c21e76b2d7a4bc287f0607ada1553ef16f60"
                      }
                    },
                    "netowl": {
                      "version": '4.9.9.0'
                    },
                    "ec_nginx": {
                      "version": "1.24.0",
                      "ngx_geoip2_version": "3.4", 
                      "geoip2_module": {
                        "version": "3.4"
                      }
                    },
                    "ec_rails_keys": {
                      "s3_files": [
                        "verify_valid_production.iv",
                        "verify_valid_production.key"
                      ]
                    },
                    "apps": {
                              "console": {
                                "name": "console",
                                "server_names": [
                                  "console.echecks.com"
                                ],
                                "server_port": "19006"
                              },
                              "donations": {
                                "name": "donations",
                                  "server_names": [
                                    "donations.echecks.com"
                                  ],
                                  "server_port": "19005"
                              },
                              "retrievals": {
                                "name": "retrievals",
                                "server_names": [
                                  "retrievals.echecks.com"
                                ],
                                "server_port": "19004"
                              },
                              "onboard": {
                                "name": "onboard",
                                "server_names": [
                                  "onboard.echecks.com"
                                ],
                                "server_port": "19003"
                              },
                              "registrar": {
                                "name": "registrar",
                                "server_names": [
                                  "registrar.echecks.com"
                                ],
                                "server_port": "19002"
                              },
                              "verifyvalid": {
                                "name": "verifyvalid",
                                "server_names": [
                                  "api.echecks.com",
                                  "internal-api.echecks.com",
                                  "my.echecks.com",
                                  "payme.echecks.com",
                                  ".echecks.com"
                                ],
                                "server_port": "19001"
                              }
                            }
