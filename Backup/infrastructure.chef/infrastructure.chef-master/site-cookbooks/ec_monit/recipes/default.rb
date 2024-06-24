#
# Cookbook Name:: ec_monit
# Recipe:: default
#

apt_package 'monit' do
  action :install
end

template "/etc/monit/monitrc" do
  owner "root"
  group "root"
  mode 0700
  source 'monitrc.erb'
  action :create
  notifies :run, 'execute[reload_monit]', :delayed
end

#/data/monit.d
directory "/etc/monit.d" do
  owner "root"
  group "root"
  mode 0755
end

template "/etc/monit.d/alerts.monitrc" do
  owner "root"
  group "root"
  mode 0700
  source 'alerts.monitrc.erb'
  action :create_if_missing
end

service 'monit' do
  action :nothing
  #only_if '/etc/init.d/monit status'
end

template "/usr/local/bin/monit" do
  owner "root"
  group "root"
  mode 0700
  source 'monit.erb'
  variables({
      :nofile => 16384
  })
  action :create_if_missing
  notifies :stop, 'service[monit]', :before
  notifies :start, 'service[monit]', :delayed
end

execute 'reload_monit' do
  user 'root'
  command 'monit reload'
  action :nothing
end
