apps = node['apps'].nil? ? node['ec_unicorn']['app_name'] : node['apps']
app_environment = node['rails_env'].nil? ? node['ec_unicorn']['service']['app_environment'] : node['rails_env']
unicorn_config = node['ec_unicorn']['service']

apps.each_pair do |config,app|
  app_name = app[:name]

  service_name = "unicorn_master_#{app_name}"
  deploy_to = "#{node['ec_unicorn']['deploy_path']}#{app_name}"
  configs = "#{deploy_to}/shared/config"

  execute "daemon-reload for #{service_name}" do
    command 'systemctl daemon-reload'
    action :nothing
  end

  pidfile = "#{deploy_to}/shared/pids/unicorn_#{app_name}.pid"

  env_vars = {
    "bundle_gemfile": "#{deploy_to}#{unicorn_config['bundle_gemfile']}",
    "lc_all": unicorn_config['locale'],
    "language": unicorn_config['locale'],
    "lang": unicorn_config['locale'],
    "rails_env": app_environment,
    "rack_env": app_environment,
    "merb_env": app_environment,
    "gem_home": "#{deploy_to}/#{unicorn_config['gem_home']}",
    "app_root": "#{deploy_to}/current",
    "user": unicorn_config['user']
  }.merge!(unicorn_config['custom_env_vars'])

  template "#{configs}/env" do
    source 'env.erb'
    mode '0644'
    variables(
      environment_vars: env_vars
    )
    action :create
    only_if {  Dir.exists?(configs) }
  end

  template "/etc/systemd/system/#{service_name}.service" do
    mode   0644
    source 'etc/systemd/system/service.erb'
    variables(
      bundle:         node['ec_unicorn']['service']['bundle'],
      working_dir:    "#{deploy_to}#{unicorn_config['working_dir']}",
      config:         "#{configs}/unicorn.rb",
      app_environment: app_environment,
      environment_file_location: "#{configs}/env",
      pidfile:        pidfile,
      service_name:   service_name,
      user:           unicorn_config['user']
    )

    notifies :run, "execute[daemon-reload for #{service_name}]", :immediately
  end
end

include_recipe 'ec_monit_config::unicorn'
