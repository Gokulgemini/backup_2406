{
  "name": "test",
  "default_attributes": {
    "ec_ruby": {
      "version": "2.2.2"
    }
  },
  "run_list": [
    "recipe[ec_security_updates]",
    "recipe[ec_aws_cli]",
    "recipe[ec_custom_fonts]",
    "recipe[ec_databags]",
    "recipe[ec_git_config]",
    "recipe[ec_monit]",
    "recipe[ec_memcached]",
    "recipe[ec_ruby]",
    "recipe[ec_rails_keys]",
    "recipe[ec_nginx]",
    "recipe[ec_cloudwatch]",
    "recipe[ec_newrelic_infra]",
    "recipe[ec_monit_config]"
  ]
}
