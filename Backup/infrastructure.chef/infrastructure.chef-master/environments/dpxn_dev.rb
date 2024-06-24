name 'dpxn_dev'
description 'DPXN Dev Chef Environment'
override_attributes "vpc_env": "dev",
                    "node_env": "dev",
                    "app_env": "dev",
                    "dynatrace_oneagent": {
                      "host_group": "dpxn_app_dev"
                    },
                    "dotnet": {
                      "sdk_version": "6.0.127-0ubuntu1~22.04.1",
                      "packages_source": "https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb",
                      "sdk_release": "6.0"
                    }
