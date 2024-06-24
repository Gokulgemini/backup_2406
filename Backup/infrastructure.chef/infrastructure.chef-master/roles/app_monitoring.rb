name 'app_monitoring'
description 'configures monitoring app role'
run_list 'role[config_monitoring]',
         'recipe[ec_unicorn]',
         'recipe[ec_nginx::server]'
