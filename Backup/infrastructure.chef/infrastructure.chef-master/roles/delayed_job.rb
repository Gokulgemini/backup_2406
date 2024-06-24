name "delayed_job"

run_list  "role[app_base]",
          "role[config_verifyvalid]",
          "role[config_registrar]",
          "role[config_onboard]",
          "role[config_monitoring]",
          "role[config_donations]",
          "role[config_console]",
          "role[delayed_job_server]",
          "recipe[ec_memcached::app_configure]",
          "role[sftp_keys]"

override_attributes "server_role": "delayed_job"
