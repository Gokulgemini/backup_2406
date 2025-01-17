

package_version = node['ec_newrelic_infra']['package_version']

deb_file = "newrelic-infra_sysv_#{package_version}_sysv_amd64.deb"
deb_url = "http://download.newrelic.com/infrastructure_agent/linux/apt/pool/main/n/newrelic-infra/#{deb_file}"
data_file = "data.tar.gz"

tmp_path = "/tmp"
local_work_path = File.join(tmp_path, "newrelic_infra")
local_deb_path = File.join(local_work_path, deb_file)
local_tar_path = File.join(local_work_path, data_file)

daemon_already_installed = "ls /usr/bin/newrelic-infra && /usr/bin/newrelic-infra -version | grep #{package_version}"

directory local_work_path do
  owner "root"
  group "root"
  mode 0755
end

remote_file local_deb_path do
  source deb_url
  mode 0644
  not_if "ls #{local_deb_path}"
end

execute "Extract deb file" do
  command "ar x #{deb_file}"
  cwd local_work_path
  user "root"
  action :run
  not_if daemon_already_installed
end

execute "Install data.tar.gz" do
  command "tar zxvf #{local_tar_path} -C /"
  cwd local_work_path
  user "root"
  action :run
  not_if daemon_already_installed
end

template "/etc/init.d/newrelic-infra" do
  source "newrelic-infra.erb"
  owner "root"
  group "root"
  mode 0755
  backup false
  variables({
    :logfile => node['newrelic_infra']['logfile']
  })
end

if node['newrelic_infra']['use_newrelic_addon']
  license_key = newrelic_license_key
else
  license_key = node['newrelic_infra']['license_key']
end

display_name = node['newrelic_infra']['display_name']

custom_attributes = { "environment": node['dna']['environment']['name'], "role": node['dna']['instance_role'] }

Chef::Log.info(custom_attributes)
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
    :dns_name_resolution => false
  })
end


include_recipe "ec_monit_config::newrelic_infra"
