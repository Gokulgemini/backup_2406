name 'mpx_sandbox'
description 'The mpx_sandbox chef environment infrastucture deployment of the mpx_sandbox rails environment to the app environment infrastucture named mpx_sandbox within the nonprod vpc'

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
  'ec_nginx' =>  '~> 1.4.0',
  'ec_prompt' =>  '~> 1.2.1',
  'ec_rails_keys' =>  '~> 1.1.0',
  'ec_ruby' =>  '~> 1.0.18',
  'ec_unicorn' =>  '~> 1.0.23',
  'users' =>  '~> 5.4.0',
  'ec_users' =>  '~> 1.0.9',
  'ec_deluxe_sudoers' => '~> 0.1.0'
)
    override_attributes "vpc_env": "nonprod",
                        "rails_env": "mpx_sandbox",
                        "domain": "mpx-sandbox.echecks.com",
                        "apps": {
                                  "console": {
                                    "name": "console",
                                    "server_names": [
                                      "console.mpx-sandbox.echecks.com"
                                    ],
                                    "server_port": "19006"
                                  },
                                  "donations": {
                                    "name": "donations",
                                      "server_names": [
                                        "donations.mpx-sandbox.echecks.com"
                                      ],
                                      "server_port": "19005"
                                  },
                                  "monitoring": {
                                    "name": "monitoring",
                                    "server_names": [
                                      "monitoring.mpx-sandbox.echecks.com"
                                    ],
                                    "server_port": "19004"
                                  },
                                  "onboard": {
                                    "name": "onboard",
                                    "server_names": [
                                      "onboard.mpx-sandbox.echecks.com"
                                    ],
                                    "server_port": "19003"
                                  },
                                  "registrar": {
                                    "name": "registrar",
                                    "server_names": [
                                      "registrar.mpx-sandbox.echecks.com"
                                    ],
                                    "server_port": "19002"
                                  },
                                  "verifyvalid": {
                                    "name": "verifyvalid",
                                    "server_names": [
                                      "api.mpx-sandbox.echecks.com",
                                      "internal-api.mpx-sandbox.echecks.com",
                                      "app.mpx-sandbox.echecks.com",
                                      "my.mpx-sandbox.echecks.com",
                                      "mpx-sandbox.echecks.com"
                                    ],
                                    "server_port": "19001"
                                  }
                                }
