name 'dpxn_xuat'
description 'DPXN XUAT Chef Environment'
override_attributes "vpc_env": "prod",
                    "node_env": "xuat",
                    "app_env": "xuat",
                    "dynatrace_oneagent": {
                      "host_group": "dpxn_app_xuat"
                    },
                    "dotnet": {
                      "sdk_version": "6.0.125-0ubuntu1~22.04.1",
                      "packages_source": "https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb",
                      "sdk_release": "6.0"
                    }
