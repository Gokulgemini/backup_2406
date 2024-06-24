name 'app_console'
description 'configures verifyvalid app role'
run_list 'role[config_console]',
         'recipe[ec_unicorn]',
         'recipe[ec_nginx::server]'

override_attributes "ec_unicorn": {
  "console": {
    "semantic_logger": true,
    "number_of_workers": 2
  }
}
