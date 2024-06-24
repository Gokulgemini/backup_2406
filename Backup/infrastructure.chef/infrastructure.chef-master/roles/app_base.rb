name 'app_base'
description 'configures common app servers'
run_list "role[common]",
         "recipe[ec_pdftk]",
         "recipe[ec_custom_fonts]",
         "recipe[ec_ruby]",
         "recipe[ec_users::deploy_user]",
         "recipe[ec_rails_keys]",
         "recipe[ec_cron]",
         "recipe[ec_prompt]"

override_attributes "ec_unicorn": {
  "service": {
    "custom_env_vars": {
      "ONESDK_LICENSE": "RjYDY=vMYLOEwIzQDwMzxAwM"
    }
  }
}
