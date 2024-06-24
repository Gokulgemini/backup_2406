name 'config_registrar'
description 'configures verifyvalid app role'
run_list 'recipe[ec_app_config]'

override_attributes "ec_unicorn":
                    {
                      "registrar":{
                        "semantic_logger": true
                      }
                    }
