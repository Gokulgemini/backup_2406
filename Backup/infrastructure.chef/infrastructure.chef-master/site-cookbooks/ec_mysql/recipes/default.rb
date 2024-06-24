#
# Cookbook:: ec_mysql
# Recipe:: default
#
# Copyright:: 2019, The Authors, All Rights Reserved.
# installing mysql version 5.7.0
# on port 3306

db_config = node['ec_mysql']

db_creds = data_bag_item(node['chef_environment'],'database')['credentials'].select{|creds| creds['username'].eql? 'root'}.first

repl_creds = data_bag_item(node['chef_environment'],'database')['credentials'].select{|creds| creds['replication_user'].eql? 'repl'}.first


# here is where we figure out if we are a master or a slave.
require 'aws-sdk-ec2'

ec2 = Aws::EC2::Resource.new(region: "#{node['ec2']['region']}")

ec2.instances({ instance_ids: ["#{node['ec2']['instance_id']}"],}).each do |i|

  # find the instance tags if we're a DBRole of master we will do things differnt.
  role = i.tags.select{ |tag| tag[:key].eql? 'DBRole'}.first[:value]

  case role

  when 'master'
    include_recipe '::master'
    include_recipe '::create_replication_user'


  when 'slave'
    include_recipe '::slave'
    include_recipe '::start_replication'
  end

end

mysql_client 'default' do
  action :create
end
