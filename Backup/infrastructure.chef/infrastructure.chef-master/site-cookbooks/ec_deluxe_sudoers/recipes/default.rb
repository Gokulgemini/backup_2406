#
# Cookbook:: ec_deluxe_sudoers
# Recipe:: default
#
# Copyright:: 2020, The Authors, All Rights Reserved.
#
private_keys = data_bag_item(node['ec_deluxe_sudoers']['databag'], node['ec_deluxe_sudoers']['item'])

# Security needs apt package for cracklib-installed for some passwordy stuff.
apt_package 'cracklib-runtime' do
  action :install
end

directory '/var/adm' do
  action :create
end

file '/var/adm/sudo.log' do
  mode '0644'
  owner 'root'
  group 'root'
  action :touch
end

directory '/root/.ssh' do
  mode '0700'
  owner 'root'
  group 'root'
end

file '/root/.ssh/sync_sudo' do
  content private_keys['private_key']
  mode '0400'
  owner 'root'
  group 'root'
end

directory '/root/.sync' do
  action :create
end

file '/root/.sync/sudoers' do
  content '.'
  owner 'root'
  group 'root'
  not_if { ::File.exists?('/root/.sync/sudoers')}
end

template '/root/.sync/sync_sudo.sh' do
  source 'sync_sudo.sh.erb'
  variables(
    :sudo_ssh_user => node['ec_deluxe_sudoers']['sudo_ssh_user'],
    :sudo_ssh_host => node['ec_deluxe_sudoers']['sudo_ssh_host'],
    :sudo_ssh_source => node['ec_deluxe_sudoers']['sudo_ssh_source'])
  owner 'root'
  group 'root'
  mode '0755'
  action :create
end

# sync the sudoers file
execute 'sync_sudoers_file' do
  command '/bin/bash /root/.sync/sync_sudo.sh'
end

cron 'sync_sudo.sh' do
  hour '*'
  minute '0'
  command '/bin/bash /root/.sync/sync_sudo.sh'
  user 'root'
  only_if {File.exists?('/root/.sync/sync_sudo.sh')}
end
