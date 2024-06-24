name 'dpx_resque'
description 'Setups up the dpx_resque role'
run_list 'role[app_base]',
          'recipe[ec_nginx]',
          'role[app_verifyvalid]',
         'recipe[ec_monit_config::resque]'

override_attributes "server_role": "resque"
