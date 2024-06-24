name 'app_base_web'
description 'configures base settings for a web tier app server'
run_list "role[app_base]",
         "recipe[ec_memcached]",
         "recipe[ec_nginx]"
