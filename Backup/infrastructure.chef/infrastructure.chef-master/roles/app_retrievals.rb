name 'app_retrievals'
description 'configures retrievals app role'
run_list 'role[retrievals]',
         'role[config_retrievals]',
         'recipe[ec_memcached::app_configure]',
         'recipe[ec_unicorn]',
         'recipe[ec_nginx::server]'

override_attributes "ec_unicorn": {
                      "service": {
                        "custom_env_vars": {
                           "ENGINE_RETRIEVALS": 1
                        }
                      },
                      "retrievals": {
                        "semantic_logger": true,
                        "number_of_workers": 12
                      }
                    }
