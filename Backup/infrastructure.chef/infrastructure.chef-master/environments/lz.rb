name 'lz'
description 'The lz chef environment infrastucture deployment of the lz rails environment to the app environment infrastucture named lz within the dev vpc'

override_attributes "vpc_env": "dev",
                    "rails_env": "lz",
                    "domain": "lz.echecks.com",
                    "apps": {
                              "console": {
                                "name": "console",
                                "server_names": [
                                  "console.lz.echecks.com"
                                ],
                                "server_port": "19006",
                                "configurations": {
                                  "database": "verifyvalid",
                                  "db_host": "dbmaster-dev-lz.gs-echecks.com",
                                  "es_host": "echecks-es-dev-lz.gs-echecks.com"
                                }
                              },
                              "donations": {
                                "name": "donations",
                                  "server_names": [
                                    "donations.lz.echecks.com"
                                  ],
                                  "server_port": "19005",
                                  "configurations": {
                                    "database": "donations",
                                    "db_host": "dbmaster-dev-lz.gs-echecks.com"
                                  }
                              },
                              "monitoring": {
                                "name": "monitoring",
                                "server_names": [
                                  "monitoring.lz.echecks.com"
                                ],
                                "server_port": "19004",
                                "configurations": {
                                  "database": "monitoring",
                                  "db_host": "dbmaster-dev-lz.gs-echecks.com"
                                }
                              },
                              "onboard": {
                                "name": "onboard",
                                "server_names": [
                                  "onboard.lz.echecks.com"
                                ],
                                "server_port": "19003",
                                "configurations": {
                                  "database": "vv_onboard",
                                  "db_host": "dbmaster-dev-lz.gs-echecks.com"
                                }
                              },
                              "registrar": {
                                "name": "registrar",
                                "server_names": [
                                  "registrar.lz.echecks.com"
                                ],
                                "server_port": "19002",
                                "configurations": {
                                  "database": "Registrar",
                                  "db_host": "dbmaster-dev-lz.gs-echecks.com"
                                }
                              },
                              "verifyvalid": {
                                "name": "verifyvalid",
                                "server_names": [
                                  "api.lz.echecks.com",
                                  "internal-api.lz.echecks.com",
                                  "app.lz.echecks.com",
                                  "my.lz.echecks.com",
                                  "lz.echecks.com",
                                  "echecks.com"
                                ],
                                "server_port": "19001",
                                "configurations": {
                                  "database": "verifyvalid",
                                  "db_host": "dbmaster-dev-lz.gs-echecks.com",
                                  "es_host": "echecks-es-dev-lz.gs-echecks.com"
                                }
                              }
                            }
