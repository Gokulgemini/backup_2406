#
# Cookbook:: ec_mysql
# Recipe:: start_replication
#
# Copyright:: 2019, The Authors, All Rights Reserved.
# installing mysql version 5.7.0
# on port 3306

db_config = node['ec_mysql']

db_creds = data_bag_item(node['chef_environment'],'database')['credentials'].select{|creds| creds['username'].eql? 'root'}.first
repl_creds = data_bag_item(node['chef_environment'],'database')['credentials'].select{|creds| creds['replication_user'].eql? 'repl'}.first

ruby_block 'change_master_to' do # libraries/helpers.rb
  block { change_master_to(db_creds['password'], "dbmaster-#{ENV['ENVIRONMENT']}-#{ENV['APP_ENVIRONMENT']}.gs-echecks.com", repl_creds['replication_user'], repl_creds['replication_password']) } # libraries/helpers.rb
  sensitive true
  not_if "/usr/bin/mysql -u root -h 127.0.0.1 -P 3307 -p#{Shellwords.escape(db_creds['password'])} -e 'SHOW SLAVE STATUS\G' | grep Slave_IO_State"
  action :run
end

# start replication on slave
bash 'Start Replication' do
  code <<-EOF
  /usr/bin/mysqldump -h dbmaster--#{ENV['ENVIRONMENT']}-#{ENV['APP_ENVIRONMENT']}.gs-echecks.com -u#{Shellwords.escape(repl_creds['replication_user'])} -p#{Shellwords.escape(repl_creds['replication_password'])} --all-databases --skip-lock-tables --single-transaction --flush-logs --hex-blob --apply-slave-statements --master-data | /usr/bin/mysql -uroot -p#{Shellwords.escape(db_creds['password'])}
  EOF
  sensitive true
  not_if "/usr/bin/mysql -S /var/run/mysqld/mysqld.sock -u root -p#{Shellwords.escape(db_creds['password'])} -e 'SHOW SLAVE STATUS\\G' | grep Slave_IO_State"
  action :run
end
