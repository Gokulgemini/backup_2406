#
# Cookbook:: ec_common
# Recipe:: default
#
# Copyright:: 2020, The Authors, All Rights Reserved.

package 'aide' do
  action :remove
end

package 'aide-common' do
  action :remove
end

package 'aide-xen' do
  action :remove
end

file '/etc/cron.daily/aide' do
  action :delete
end

# Remove aide from crontab
delete_lines 'remove aide comment fron  crontab' do
  path '/etc/crontab'
  pattern /^#Ansible.*/
end

delete_lines 'remove aide fron  crontab' do
  path '/etc/crontab'
  pattern /^*.aide\.wrapper.*$/
end
