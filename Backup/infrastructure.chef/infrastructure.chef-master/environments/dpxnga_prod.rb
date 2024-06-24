name 'dpxnga_prod'
description 'DPXN GA PROD Chef Environment'
override_attributes "vpc_env": "prod",
                    "node_env": "production",
                    "app_env": "production",
                    "dynatrace_oneagent": {
                      "host_group": "dpxnga_app_production"
                    },
                    "dotnet": {
                      "sdk_version": "6.0.125-0ubuntu1~22.04.1",
                      "packages_source": "https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb",
                      "sdk_release": "6.0"
                    }
