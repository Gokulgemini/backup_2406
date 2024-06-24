name 'training'
description 'The training chef environment infrastucture deployment of the training rails environment to the app environment infrastucture named training within the nonprod vpc'
cookbook_versions(
  'aws' => '~> 8.1.1',
  's3_file' => '~> 2.8.5',
  'ec_app_config' => '~> 1.1.5',
  'ec_monit' =>  '~> 1.1.0',
  'ec_monit_config' =>  '~> 1.1.0',
  'ec_clamav' =>  '~> 1.0.27',
  'ec_cloudwatch' =>  '~> 1.0.3',
  'ec_cron' =>  '~> 1.3.1',
  'ec_security_updates' =>  '~> 1.0.38',
  'ec_custom_fonts' =>  '~> 1.0.3',
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
  'ec_nginx' =>  '~> 1.3.1',
  'ec_prompt' =>  '~> 1.2.1',
  'ec_rails_keys' =>  '~> 1.1.0',
  'ec_ruby' =>  '~> 1.0.18',
  'ec_unicorn' =>  '~> 1.0.23',
  'users' =>  '~> 5.4.0',
  'ec_users' =>  '~> 1.0.9',
  'ec_deluxe_sudoers' => '~> 0.1.0'
)

override_attributes "vpc_env": "nonprod",
                    "rails_env": "training",
                    "domain": "training.echecks.com",
                    "ec_ruby": {
                      "version": "2.7.5"
                    },
                    "nodejs": {
                      "version": '16.13.1',
                      "binary": {
                        "checksum": "5f80197d654fd0b749cdeddf1f07a5eac1fcf6b423a00ffc8f2d3bea9c6dc8d1"
                      }
                    },
                    "apps": {
                              "console": {
                                "name": "console",
                                "server_names": [
                                  "console.training.echecks.com"
                                ],
                                "server_port": "19006",
                                "configurations": {
                                  "database": "verifyvalid",
                                  "db_host": "db-nonprod-training.gs-echecks.com",
                                  "es_host": "echecks-es-nonprod-training.gs-echecks.com"
                                }
                              },
                              "donations": {
                                "name": "donations",
                                  "server_names": [
                                    "donations.training.echecks.com"
                                  ],
                                  "server_port": "19005",
                                  "configurations": {
                                    "database": "donations",
                                    "db_host": "db-nonprod-training.gs-echecks.com"
                                  }
                              },
                              "monitoring": {
                                "name": "monitoring",
                                "server_names": [
                                  "monitoring.training.echecks.com"
                                ],
                                "server_port": "19004",
                                "configurations": {
                                  "database": "monitoring",
                                  "db_host": "db-nonprod-training.gs-echecks.com"
                                }
                              },
                              "onboard": {
                                "name": "onboard",
                                "server_names": [
                                  "onboard.training.echecks.com"
                                ],
                                "server_port": "19003",
                                "configurations": {
                                  "database": "vv_onboard",
                                  "db_host": "db-nonprod-training.gs-echecks.com"
                                }
                              },
                              "registrar": {
                                "name": "registrar",
                                "server_names": [
                                  "registrar.training.echecks.com"
                                ],
                                "server_port": "19002",
                                "configurations": {
                                  "database": "Registrar",
                                  "db_host": "db-nonprod-training.gs-echecks.com"
                                }
                              },
                              "verifyvalid": {
                                "name": "verifyvalid",
                                "server_names": [
                                  "api.training.echecks.com",
                                  "internal-api.training.echecks.com",
                                  "app.training.echecks.com",
                                  "my.training.echecks.com",
                                  "training.echecks.com",
                                  "payme.training.echecks.com"
                                ],
                                "server_port": "19001",
                                "configurations": {
                                  "database": "verifyvalid",
                                  "db_host": "db-nonprod-training.gs-echecks.com",
                                  "es_host": "echecks-es-nonprod-training.gs-echecks.com"
                                }
                              }
                            }
