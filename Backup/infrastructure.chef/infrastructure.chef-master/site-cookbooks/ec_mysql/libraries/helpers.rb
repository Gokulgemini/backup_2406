require 'chef/mixin/shell_out'
require 'shellwords'
include Chef::Mixin::ShellOut

def change_master_to(root_pass, master_host, replication_user, replication_password)
  query = ' CHANGE MASTER TO'
  query << " MASTER_HOST='#{master_host}',"
  query << " MASTER_USER='#{replication_user}',"
  query << " MASTER_PASSWORD='#{replication_password}',"
  query << ' MASTER_PORT=3306;'
  query << ' START SLAVE;'
  shell_out!("echo \"#{query}\" | /usr/bin/mysql -u root -p#{Shellwords.escape(root_pass)}")
end
