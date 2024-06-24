name 'development'
description 'The development chef environment infrastucture deployment of the development rails environment to the app environment infrastucture named development within the dev vpc'

override_attributes "vpc_env": "dev",
                    "rails_env": "development",
                    "domain": "dev.echecks.com",
                    "apps": {
                              "console": {
                                "name": "console",
                                "server_names": [
                                  "console.dev.echecks.com"
                                ],
                                "server_port": "19006",
                                "configurations": {
                                  "database": "verifyvalid",
                                  "db_host": "dbmaster-dev-dev.gs-echecks.com"
                                }
                              },
                              "donations": {
                                "name": "donations",
                                  "server_names": [
                                    "donations.dev.echecks.com"
                                  ],
                                  "server_port": "19005",
                                  "configurations": {
                                    "database": "donations",
                                    "db_host": "dbmaster-dev-dev.gs-echecks.com"
                                  }
                              },
                              "monitoring": {
                                "name": "monitoring",
                                "server_names": [
                                  "monitoring.dev.echecks.com"
                                ],
                                "server_port": "19004",
                                "configurations": {
                                  "database": "monitoring",
                                  "db_host": "dbmaster-dev-dev.gs-echecks.com"
                                }
                              },
                              "onboard": {
                                "name": "onboard",
                                "server_names": [
                                  "onboard.dev.echecks.com"
                                ],
                                "server_port": "19003",
                                "configurations": {
                                  "database": "vv_onboard",
                                  "db_host": "dbmaster-dev-dev.gs-echecks.com"
                                }
                              },
                              "registrar": {
                                "name": "registrar",
                                "server_names": [
                                  "registrar.dev.echecks.com"
                                ],
                                "server_port": "19002",
                                "configurations": {
                                  "database": "Registrar",
                                  "db_host": "dbmaster-dev-dev.gs-echecks.com"
                                }
                              },
                              "verifyvalid": {
                                "name": "verifyvalid",
                                "server_names": [
                                  "api.dev.echecks.com",
                                  "internal-api.dev.echecks.com",
                                  "app.dev.echecks.com",
                                  "my.dev.echecks.com",
                                  "dev.echecks.com",
                                  "echecks.com"
                                ],
                                "server_port": "19001",
                                "configurations": {
                                  "database": "verifyvalid",
                                  "db_host": "dbmaster-dev-dev.gs-echecks.com",
                                  "es_host": "vpc-echecks-dev-db-es-w63lwi5k7wjum2j5dzhvfyqmue.us-east-1.es.amazonaws.com"
                                }
                              }
                            }
