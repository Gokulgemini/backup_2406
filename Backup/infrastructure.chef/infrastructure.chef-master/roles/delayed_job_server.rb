name "delayed_job_server"

run_list  "recipe[ec_app_config]",
          "recipe[ec_cron::delayed_job]",
          "recipe[ec_monit_config::delayed_job]"
