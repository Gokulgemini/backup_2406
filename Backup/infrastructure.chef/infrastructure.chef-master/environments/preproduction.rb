name 'preproduction'
description 'The preproduction chef environment infrastucture deployment of the preproduction rails environment to the app environment infrastucture named preproduction within the dev vpc'

cookbook 'ec_prompt', '~> 1.2.1'
cookbook 'ec_common', '~> 0.1.0'
cookbook 'line', '~> 2.8.1'
cookbook 'ec_security_updates', '~> 1.0.41'
cookbook 'ec_pdftk', '~> 1.0.0'
cookbook 'ec_monit', '~> 1.1.0'
cookbook 'ec_nginx', '~> 1.4.6'
cookbook 'ec_users', '~> 1.0.10'
cookbook 'users', '~> 5.4.0'
cookbook 'ec_clamav', '~> 1.0.27'
cookbook 'ec_monit_config', '~> 1.2.1'
cookbook 'ec_cloudwatch', '~> 1.1.2'
cookbook 'ec_newrelic_infra', '~> 1.0.7'
cookbook 'ec_deluxe_sudoers', '~> 0.1.2'
cookbook 'ec_custom_fonts', '~> 1.0.5'
cookbook 'ec_ruby', '~> 1.0.20'
cookbook 'brightbox-ruby', '~> 1.2.2'
cookbook 'apt', '~> 7.3.0'
cookbook 'ec_rails_keys', '~> 1.1.0'
cookbook 's3_file', '~> 2.8.5'
cookbook 'ec_app_config', '~> 1.1.23'
cookbook 'aws', '~> 8.1.1'
cookbook 'ec_cron', '~> 1.3.4'
cookbook 'ec_memcached', '~> 0.1.3'
cookbook 'memcached', '~> 5.1.1'
cookbook 'runit', '~> 5.1.2'
cookbook 'packagecloud', '~> 1.0.1'
cookbook 'yum-epel', '~> 3.3.0'
cookbook 'dynatrace_oneagent', '~> 1.0.0'

override_attributes "vpc_env": "dev",
                    "rails_env": "preproduction",
                    "app_env": "preproduction",
                    "domain": "pre.echecks.com",
                    "dynatrace_oneagent": {
                      "host_group": "echecks_app_preproduction"
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
                        "semantic_logger": true
                      },
                      "console":{
                        "semantic_logger": true
                      },
                      "registrar":{
                        "semantic_logger": true
                      }
                    },
                    "apps": {
                              "console": {
                                "name": "console",
                                "server_names": [
                                  "console.pre.echecks.com"
                                ],
                                "server_port": "19006",
                                "configurations": {
                                  "database": "verifyvalid",
                                  "db_host": "db-dev-preproduction.gs-echecks.com",
                                  "es_host": "echecks-es-dev-preproduction.gs-echecks.com"
                                },
                                "semantic_logger": true
                              },
                              "donations": {
                                "name": "donations",
                                  "server_names": [
                                    "donations.pre.echecks.com"
                                  ],
                                  "server_port": "19005",
                                  "configurations": {
                                    "database": "donations",
                                    "db_host": "db-dev-preproduction.gs-echecks.com"
                                  }
                              },
                              "retrievals": {
                                "name": "retrievals",
                                "server_names": [
                                  "retrievals.pre.echecks.com"
                                ],
                                "server_port": "19004",
                                "configurations": {
                                  "database": "verifyvalid",
                                  "db_host": "db-dev-preproduction.gs-echecks.com"
                                }
                              },
                              "onboard": {
                                "name": "onboard",
                                "server_names": [
                                  "onboard.pre.echecks.com"
                                ],
                                "server_port": "19003",
                                "configurations": {
                                  "database": "vv_onboard",
                                  "db_host": "db-dev-preproduction.gs-echecks.com"
                                }
                              },
                              "registrar": {
                                "name": "registrar",
                                "server_names": [
                                  "registrar.pre.echecks.com"
                                ],
                                "server_port": "19002",
                                "configurations": {
                                  "database": "Registrar",
                                  "db_host": "db-dev-preproduction.gs-echecks.com"
                                },
                                "semantic_logger": true
                              },
                              "verifyvalid": {
                                "name": "verifyvalid",
                                "server_names": [
                                  "api.pre.echecks.com",
                                  "internal-api.pre.echecks.com",
                                  "app.pre.echecks.com",
                                  "my.pre.echecks.com",
                                  "pre.echecks.com",
                                  "payme.pre.echecks.com"
                                ],
                                "server_port": "19001",
                                "configurations": {
                                  "database": "verifyvalid",
                                  "db_host": "db-dev-preproduction.gs-echecks.com",
                                  "es_host": "echecks-es-dev-preproduction.gs-echecks.com"
                                },
                                "semantic_logger": true
                              }
                            }