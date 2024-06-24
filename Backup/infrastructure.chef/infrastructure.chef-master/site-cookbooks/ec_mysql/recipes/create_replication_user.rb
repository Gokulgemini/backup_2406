#
# Cookbook:: ec_mysql
# Recipe:: create_replication_user
#
# Copyright:: 2019, The Authors, All Rights Reserved.
# installing mysql version 5.7.0
# on port 3306

db_config = node['ec_mysql']

db_creds = data_bag_item(node['chef_environment'],'database')['credentials'].select{|creds| creds['username'].eql? 'root'}.first
repl_creds = data_bag_item(node['chef_environment'],'database')['credentials'].select{|creds| creds['replication_user'].eql? 'repl'}.first


# Create user repl on master
    bash 'create replication user' do
      code <<-EOF
      /usr/bin/mysql -S /var/run/mysqld/mysqld.sock -u root -p#{Shellwords.escape(db_creds['password'])} -D mysql -e "CREATE USER '#{Shellwords.escape(repl_creds['replication_user'])}'@'%' IDENTIFIED BY '#{Shellwords.escape(repl_creds['replication_password'])}';"
      /usr/bin/mysql -S /var/run/mysqld/mysqld.sock -u root -p#{Shellwords.escape(db_creds['password'])} -D mysql -e "GRANT REPLICATION SLAVE ON *.* TO '#{Shellwords.escape(repl_creds['replication_user'])}'@'%';"
      /usr/bin/mysql -S /var/run/mysqld/mysqld.sock -u root -p#{Shellwords.escape(db_creds['password'])} -D mysql -e "GRANT REPLICATION CLIENT ON *.* TO '#{Shellwords.escape(repl_creds['replication_user'])}'@'%';"
      /usr/bin/mysql -S /var/run/mysqld/mysqld.sock -u root -p#{Shellwords.escape(db_creds['password'])} -D mysql -e "GRANT RELOAD ON *.* TO '#{Shellwords.escape(repl_creds['replication_user'])}'@'%';"
      /usr/bin/mysql -S /var/run/mysqld/mysqld.sock -u root -p#{Shellwords.escape(db_creds['password'])} -D mysql -e "GRANT SUPER ON *.* TO '#{Shellwords.escape(repl_creds['replication_user'])}'@'%';"
      EOF
      sensitive true
      not_if "/usr/bin/mysql -S /var/run/mysqld/mysqld.sock -u root -p#{Shellwords.escape(db_creds['password'])} -e 'select User,Host from mysql.user' | grep repl"
      action :run
    end
