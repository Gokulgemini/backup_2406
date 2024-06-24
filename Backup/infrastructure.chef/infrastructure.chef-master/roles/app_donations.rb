name 'app_donations'
description 'configures donations app role'
run_list 'role[config_donations]',
         'recipe[ec_unicorn]',
         'recipe[ec_nginx::server]'
