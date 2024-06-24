#
# Cookbook Name: ec_newrelic_infra
# Recipe:: default
#

license_key = node['ec_newrelic_infra']['license_key']
display_name = node['ec_newrelic_infra']['display_name']
custom_attributes = { "environment": node['rails_env'], "role": node['server_role'] }
dns_name_resolution = node['ec_newrelic_infra']['dns_name_resolution']

package_version = node['ec_newrelic_infra']['package_version']

Chef::Log.info(Chef::JSONCompat.to_json_pretty(custom_attributes))
template "/etc/newrelic-infra.yml" do
  source "newrelic-infra.yml.erb"
  owner "root"
  group "root"
  mode 0600
  backup false
  variables({
    :license_key => license_key,
    :display_name => display_name,
    :custom_attributes => custom_attributes,
    :dns_name_resolution => dns_name_resolution
  })
end


apt_repository 'newrelic-infra' do
  cache_rebuild      true
  arch               "amd64"
  components         ['main']
  distribution       "bionic"
  key                "https://download.newrelic.com/infrastructure_agent/gpg/newrelic-infra.gpg"
  repo_name          "newrelic-infra"
  trusted            false
  uri                "https://download.newrelic.com/infrastructure_agent/linux/apt"
  action             :add
end

apt_package 'name' do
  package_name  "newrelic-infra"
  version       package_version
  action        :install
end

# template "/etc/init.d/newrelic-infra" do
#   source "newrelic-infra.erb"
#   owner "root"
#   group "root"
#   mode 0755
#   backup false
#   variables({
#     :logfile => node['ec_newrelic_infra']['logfile']
#   })
# end

template "/etc/systemd/system/newrelic-infra.service" do
  source "newrelic-infra.service.erb"
  owner "root"
  group "root"
  mode 0644
  backup false
  variables({
    :logfile => node['ec_newrelic_infra']['logfile']
  })
  notifies :run, 'execute[daemon-reload for newrelic-infra]', :immediately
end

execute "daemon-reload for newrelic-infra" do
  command 'systemctl daemon-reload'
  action :nothing
  notifies :restart, 'service[newrelic-infra]', :immediately
end

service "newrelic-infra" do
  action :nothing
end

include_recipe "ec_monit_config::newrelic_infra"
