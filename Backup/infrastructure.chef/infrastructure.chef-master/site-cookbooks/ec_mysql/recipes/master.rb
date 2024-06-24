#
# Cookbook:: ec_mysql
# Recipe:: master
#
# Copyright:: 2019, The Authors, All Rights Reserved.
# installing mysql version 5.7.0
# on port 3306

db_creds = data_bag_item(node['chef_environment'],'database')['credentials'].select{|creds| creds['username'].eql? 'root'}.first

# master setup
mysql_service 'default' do
  port node['ec_mysql']['mysqld']['port']
  bind_address node['ec_mysql']['mysqld']['bind_address']
  pid_file node['ec_mysql']['mysqld']['pid_file']
  socket_file node['ec_mysql']['mysqld']['socket']
  data_dir node['ec_mysql']['mysqld']['datadir']
  tmp_dir node['ec_mysql']['tmpfs_mountpoint']
  version node['ec_mysql']['version']
  initial_root_password db_creds['password']
  action [:create, :start]
end

directory "#{node['ec_mysql']['db_data_mountpoint']}" do
  owner 'mysql'
  group 'mysql'
  recursive true
end

directory "#{node['ec_mysql']['db_tmpfs_mountpoint']}" do
  owner 'mysql'
  group 'mysql'
  recursive true
end

directory "#{node['ec_mysql']['db_backup_mountpoint']}" do
  owner 'mysql'
  group 'mysql'
  recursive true
end

# Now we reconfigure mysql and start it up.
mysql_config 'default' do
  instance 'default'
  config_name 'replication'
  source 'replication-master.erb'
  variables(
    :server_id => '1',
    :ec_vars_client => node['ec_mysql']['client'],
    :ec_vars_mysqladmin => node['ec_mysql']['mysqladmin'],
    :ec_vars_mysqlcheck => node['ec_mysql']['mysqlcheck'],
    :ec_vars_mysqlimport => node['ec_mysql']['mysqlimport'],
    :ec_vars_mysqlshow => node['ec_mysql']['mysqlshow'],
    :ec_vars_myisampack => node['ec_mysql']['myisampack'],
    :ec_vars_mysqld => node['ec_mysql']['mysqld'],
    :ec_vars_mysqldump => node['ec_mysql']['mysqldump'],
    :ec_vars_mysql => node['ec_mysql']['mysql'],
    :ec_vars_isamchk => node['ec_mysql']['isamchk'],
    :ec_vars_myisamchk => node['ec_mysql']['myisamchk'],
    :ec_vars_mysqlhotcopy => node['ec_mysql']['mysqlhotcopy'],
    :mysql_instance => 'default',
    :binlog_format => node['ec_mysql']['mysqld']['binlog_format'],
    :max_allowed_packet => node['ec_mysql']['mysqld']['max_allowed_packet']
  )
  notifies :restart, 'mysql_service[default]', :immediately
  action :create
end

# Setup the apparmor template
template '/etc/apparmor.d/local/usr.sbin.mysqld' do
  source 'local.usr.sbin.mysqld.erb'
  mode '0644'
  owner 'root'
  group 'root'
  variables(db_data_mountpoint: node['ec_mysql']['db_data_mountpoint'],
            db_tmpfs_mountpoint: node['ec_mysql']['db_tmpfs_mountpoint'],
            db_backup_mountpoint: node['ec_mysql']['db_backup_mountpoint'])
end

# re-enable appamor
file '/etc/apparmor.d/disable/usr.sbin.mysqld' do
  action :delete
end
# restart the service
service 'apparmor' do
  subscribes :reload, 'template[/etc/apparmor.d/local/usr.sbin.mysqld]', :immediately
end

service 'mysql' do
  action :restart
end

mysql_client 'default' do
  action :create
end

include_recipe '::create_replication_user'
