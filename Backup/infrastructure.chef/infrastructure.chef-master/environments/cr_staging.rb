name 'cr_staging'
description 'The cr_staging chef environment infrastucture deployment of the staging rails environment to the app environment infrastucture named staging within the dev vpc'

cookbook 'ec_prompt', '~> 1.2.1'
cookbook 'ec_common', '~> 0.1.0'
cookbook 'line', '~> 2.8.1'
cookbook 'ec_security_updates', '~> 1.0.41'
cookbook 'ec_pdftk', '~> 0.1.1'
cookbook 'ec_monit', '~> 1.1.0'
cookbook 'ec_nginx', '~> 1.4.0'
cookbook 'ec_users', '~> 1.0.10'
cookbook 'users', '~> 5.4.0'
cookbook 'ec_clamav', '~> 1.0.27'
cookbook 'ec_monit_config', '~> 1.2.1'
cookbook 'ec_cloudwatch', '~> 1.0.3'
cookbook 'ec_newrelic_infra', '~> 1.0.7'
cookbook 'ec_deluxe_sudoers', '~> 0.1.2'
cookbook 'ec_custom_fonts', '~> 1.0.5'
cookbook 'ec_ruby', '~> 1.0.19'
cookbook 'brightbox-ruby', '~> 1.2.2'
cookbook 'apt', '~> 7.3.0'
cookbook 'ec_rails_keys', '~> 1.1.0'
cookbook 's3_file', '~> 2.8.5'
cookbook 'ec_app_config', '~> 1.1.11'
cookbook 'aws', '~> 8.1.1'
cookbook 'ec_cron', '~> 1.3.3'
cookbook 'ec_memcached', '~> 0.1.3'
cookbook 'memcached', '~> 5.1.1'
cookbook 'runit', '~> 5.1.2'
cookbook 'packagecloud', '~> 1.0.1'
cookbook 'yum-epel', '~> 3.3.0'

override_attributes "vpc_env": "dev",
                    "rails_env": "staging",
                    "domain": "cr-staging.echecks.com",
                    "apps": {
                              "console": {
                                "name": "console",
                                "server_names": [
                                  "console.cr-staging.echecks.com"
                                ],
                                "server_port": "19006"
                              },
                              "donations": {
                                "name": "donations",
                                  "server_names": [
                                    "donations.cr-staging.echecks.com"
                                  ],
                                  "server_port": "19005"
                              },
                              "retrievals": {
                                "name": "retrievals",
                                "server_names": [
                                  "retrievals.cr-staging.echecks.com"
                                ],
                                "server_port": "19004"
                              },
                              "onboard": {
                                "name": "onboard",
                                "server_names": [
                                  "onboard.cr-staging.echecks.com"
                                ],
                                "server_port": "19003"
                              },
                              "registrar": {
                                "name": "registrar",
                                "server_names": [
                                  "registrar.cr-staging.echecks.com"
                                ],
                                "server_port": "19002"
                              },
                              "verifyvalid": {
                                "name": "verifyvalid",
                                "server_names": [
                                  "api.cr-staging.echecks.com",
                                  "internal-api.cr-staging.echecks.com",
                                  "app.cr-staging.echecks.com",
                                  "my.cr-staging.echecks.com",
                                  "cr-staging.echecks.com",
                                  "echecks.com"
                                ],
                                "server_port": "19001"
                              }
                            }
