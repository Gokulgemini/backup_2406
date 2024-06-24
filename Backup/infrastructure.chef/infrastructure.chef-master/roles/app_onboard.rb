name 'app_onboard'
description 'configures onboard app role'
run_list 'role[config_onboard]',
         'recipe[ec_unicorn]',
         'recipe[ec_nginx::server]'
