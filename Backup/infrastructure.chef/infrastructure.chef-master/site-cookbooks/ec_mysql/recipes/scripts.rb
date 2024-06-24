#
# Cookbook:: ec_mysql
# Recipe:: scripts
#
#

cookbook_file '/usr/local/bin/mysql_backup.sh' do
  source 'backup.sh'
  owner 'root'
  group 'root'
  mode '0755'
  action :create
end
