name 'db'
description 'Configures mysql masters and slaves'
run_list 'recipe[ec_monit]',
         'recipe[ec_newrelic_infra]',
         'recipe[ec_mysql::volumes]',
         'recipe[ec_mysql]',
         'recipe[ec_mysql::scripts]'

override_attributes "server_role": "db"
