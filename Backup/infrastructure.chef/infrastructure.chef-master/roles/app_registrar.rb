name 'app_registrar'
description 'configures registrar app role'
run_list 'role[config_registrar]',
         'recipe[ec_unicorn]',
         'recipe[ec_nginx::server]'

override_attributes "ec_unicorn": {
  "registrar": {
    "semantic_logger": true,
    "number_of_workers": 2
  }
}
