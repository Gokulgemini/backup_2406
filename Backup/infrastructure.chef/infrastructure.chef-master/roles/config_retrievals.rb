name 'config_retrievals'
description 'configures retrievals app role'
run_list 'recipe[ec_app_config]'

override_attributes "ec_app_config" => { "apps" => [ "retrievals" ] }
