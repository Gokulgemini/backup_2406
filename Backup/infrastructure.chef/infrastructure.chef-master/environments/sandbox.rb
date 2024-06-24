name 'sandbox'
description 'The sandbox chef environment infrastucture deployment of the sandbox rails environment to the app environment infrastucture named sandbox within the nonprod vpc'
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

override_attributes "vpc_env": "nonprod",
                    "rails_env": "sandbox",
                    "domain": "sandbox.echecks.com",
                    "dynatrace_oneagent": {
                      "host_group": "echecks_app_nonprod"
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
                    "ec_unicorn": {
                      "verifyvalid":{
                        "semantic_logger": true,
                        "number_of_workers": 2
                      },
                      "console":{
                        "semantic_logger": true,
                        "number_of_workers": 2
                      },
                      "registrar":{
                        "semantic_logger": true,
                        "number_of_workers": 2
                      }
                    },
                    "apps": {
                              "console": {
                                "name": "console",
                                "server_names": [
                                  "console.sandbox.echecks.com"
                                ],
                                "server_port": "19006",
                                "configurations": {
                                  "database": "verifyvalid",
                                  "db_host": "db-nonprod-sandbox.gs-echecks.com",
                                  "es_host": "echecks-es-nonprod-sandbox.gs-echecks.com"
                                }
                              },
                              "donations": {
                                "name": "donations",
                                  "server_names": [
                                    "donations.sandbox.echecks.com"
                                  ],
                                  "server_port": "19005",
                                  "configurations": {
                                    "database": "donations",
                                    "db_host": "db-nonprod-sandbox.gs-echecks.com"
                                  }
                              },
                              "monitoring": {
                                "name": "monitoring",
                                "server_names": [
                                  "monitoring.sandbox.echecks.com"
                                ],
                                "server_port": "19004",
                                "configurations": {
                                  "database": "monitoring",
                                  "db_host": "db-nonprod-sandbox.gs-echecks.com"
                                }
                              },
                              "onboard": {
                                "name": "onboard",
                                "server_names": [
                                  "onboard.sandbox.echecks.com"
                                ],
                                "server_port": "19003",
                                "configurations": {
                                  "database": "vv_onboard",
                                  "db_host": "db-nonprod-sandbox.gs-echecks.com"
                                }
                              },
                              "retrievals": {
                                "name": "retrievals",
                                "server_names": [
                                  "retrievals.sandbox.echecks.com"
                                ],
                                "server_port": "19004",
                                "configurations": {
                                  "database": "verifyvalid",
                                  "db_host": "db-nonprod-sandbox.gs-echecks.com"
                                }
                              },
                              "registrar": {
                                "name": "registrar",
                                "server_names": [
                                  "registrar.sandbox.echecks.com"
                                ],
                                "server_port": "19002",
                                "configurations": {
                                  "database": "Registrar",
                                  "db_host": "db-nonprod-sandbox.gs-echecks.com"
                                }
                              },
                              "verifyvalid": {
                                "name": "verifyvalid",
                                "server_names": [
                                  "api.sandbox.echecks.com",
                                  "internal-api.sandbox.echecks.com",
                                  "app.sandbox.echecks.com",
                                  "my.sandbox.echecks.com",
                                  "sandbox.echecks.com",
                                  "payme.sandbox.echecks.com"
                                ],
                                "server_port": "19001",
                                "configurations": {
                                  "database": "verifyvalid",
                                  "db_host": "db-nonprod-sandbox.gs-echecks.com",
                                  "es_host": "echecks-es-nonprod-sandbox.gs-echecks.com"
                                }
                              }
                            }
