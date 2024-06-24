name 'common'
description 'configures common serverices'
run_list  "recipe[ec_prompt]",
          "recipe[common]",
          "recipe[ec_common]",
          "recipe[ec_security_updates]",
          "recipe[ec_monit]",
          "recipe[ec_users]",
          "recipe[ec_users::deploy_user]",
          "recipe[ec_clamav]",
          "recipe[ec_cloudwatch]",
          "recipe[dynatrace_oneagent]",
          "recipe[dpx_nodejs]",
          "recipe[ec_deluxe_sudoers]"
