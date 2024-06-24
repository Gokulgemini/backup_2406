#
# Cookbook Name:: ec_memcached
# Recipe:: default
#
# Copyright (C) 2019 YOUR_NAME
#
# All rights reserved - Do Not Redistribute
#

memcached_instance 'memcached' do
  memory 128
  ulimit 1024
end
