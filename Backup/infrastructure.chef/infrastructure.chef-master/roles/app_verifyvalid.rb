name 'app_verifyvalid'
description 'configures verifyvalid app role'
run_list 'role[config_verifyvalid]',
         'recipe[ec_memcached::app_configure]',
         'recipe[ec_unicorn]',
         'recipe[ec_nginx::server]'

override_attributes "ec_unicorn": {
  "service": {
    "custom_env_vars": {
      "ENGINE_RETRIEVALS": 1
    }
  },
  "verifyvalid": {
    "semantic_logger": true,
    "number_of_workers": 8
  }
}
