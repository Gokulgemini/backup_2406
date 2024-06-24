# frozen_string_literal: true

# systemd



default['ec_unicorn']['service']['bundle']         = '/usr/local/bin/bundle'
default['ec_unicorn']['service']['working_dir']          = "/current"
default['ec_unicorn']['service']['bundle_gemfile'] = "/current/Gemfile"
default['ec_unicorn']['service']['config']         = "/shared/config/unicorn.rb"
default['ec_unicorn']['service']['app_environment']    = 'default'
#default['ec_unicorn']['service']['gem_home']       = '/data/#{fetch(:application)}/shared/bundled_gems'
default['ec_unicorn']['service']['gem_home']       = 'shared/bundled_gems'

default['ec_unicorn']['service']['locale']         = 'en_US.UTF-8'
#default['ec_unicorn']['service']['name']           = "unicorn_#{default['ec_unicorn']['app_name']}"
default['ec_unicorn']['service']['user']           = 'deploy'

default['ec_unicorn']['service']['config'] = "config/unicorn.rb"
#default['ec_unicorn']['service']['pidfile'] = "tmp/pids/unicorn_verifyvalid.pid"
#default['ec_unicorn']['service']['socket']  = "tmp/sockets/unicorn_verifyvalid.sock"

default['ec_unicorn']['service']['custom_env_vars'] = {
}
