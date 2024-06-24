name 'preproduction_clone'
description 'The preproduction_clone chef environment infrastucture deployment of the preproduction rails environment to the app environment infrastucture named preproduction_clone within the nonprod vpc'

override_attributes "vpc_env": "nonprod",
                    "rails_env": "preproduction",
                    "domain": "pre-clone.echecks.com",
                    "apps": {
                              "console": {
                                "name": "console",
                                "server_names": [
                                  "console.pre.echecks.com"
                                ],
                                "server_port": "19006",
                                "configurations": {
                                  "database": "verifyvalid",
                                  "db_host": "dbmaster-nonprod-preproduction.gs-echecks.com"
                                }
                              },
                              "donations": {
                                "name": "donations",
                                  "server_names": [
                                    "donations.pre.echecks.com"
                                  ],
                                  "server_port": "19005",
                                  "configurations": {
                                    "database": "donations",
                                    "db_host": "dbmaster-nonprod-preproduction.gs-echecks.com"
                                  }
                              },
                              "monitoring": {
                                "name": "monitoring",
                                "server_names": [
                                  "monitoring.pre.echecks.com"
                                ],
                                "server_port": "19004",
                                "configurations": {
                                  "database": "monitoring",
                                  "db_host": "dbmaster-nonprod-preproduction.gs-echecks.com"
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
                                  "db_host": "dbmaster-nonprod-preproduction.gs-echecks.com"
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
                                  "db_host": "dbmaster-nonprod-preproduction.gs-echecks.com"
                                }
                              },
                              "verifyvalid": {
                                "name": "verifyvalid",
                                "server_names": [
                                  "api.pre.echecks.com",
                                  "internal-api.pre.echecks.com",
                                  "app.pre.echecks.com",
                                  "my.pre.echecks.com",
                                  "pre.echecks.com",
                                  "echecks.com"
                                ],
                                "server_port": "19001",
                                "configurations": {
                                  "database": "verifyvalid",
                                  "db_host": "dbmaster-nonprod-preproduction.gs-echecks.com",
                                  "es_host": "vpc-echecks-pre-db-es-w63lwi5k7wjum2j5dzhvfyqmue.us-east-1.es.amazonaws.com"
                                }
                              }
                            }
