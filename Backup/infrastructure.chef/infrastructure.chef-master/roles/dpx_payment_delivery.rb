name 'dpx_payment_delivery'
description 'configures dpx payment delivery servers'
run_list "role[common]",
         "role[app_payment_delivery]",
         "recipe[dpx_dotnet]",
         "recipe[dpx_payment_delivery]"

override_attributes "server_role": "payment_delivery",
                    "ec_cloudwatch": {
                      "install": true,
                      "logs": {
                        "collect_list": [{
                          'file_path': '/var/log/**.log',
                          'log_group_name': "GS-ECHECKS-DEV-Cloudwatch-All-Logs-Group"
                        },
                        {
                          "file_path": "/data/payment_delivery/shared/log/*.log",
                          'log_group_name': "GS-ECHECKS-DEV-Cloudwatch-All-Logs-Group"
                        }
                        ]
                      }
                    },
                    "dotnet": {
                      "sdk_version": "5.0.406-1",
                      "sdk_release": "5.0",
                      "packages_source": "https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb"
                    }
