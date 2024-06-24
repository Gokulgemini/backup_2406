name "app"

run_list "role[app_base_web]",
         "role[app_console]",
         "role[app_registrar]",
         "role[app_onboard]",
         "role[app_donations]",
         "role[app_monitoring]",
         "role[app_verifyvalid]"

override_attributes "server_role": "app",
                    "dynatrace_oneagent": {
                      "host_group": "dpx_app_#{default_attributes['app_env']}"
                    }
