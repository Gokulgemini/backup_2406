user 'deploy' do
  comment 'deploy user'
  home '/home/deploy'
  manage_home true
  shell '/bin/bash'
end

#github_id = data_bag_item('github', "id_deploy")

directory "/home/deploy/.ssh" do
  owner "deploy"
  group "deploy"
  mode '0700'
  action :create
end

# file "/home/deploy/.ssh/id_github_key" do
#   content github_id['key']
#   sensitive true
#   owner "deploy"
#   group "deploy"
#   mode '0600'
#   action :create
# end

#TODO: remove this...
sudo 'deploy' do
  user 'deploy'
  nopasswd true
end

# cron_access 'deploy' do
#   action :allow
# end

s3_file "/home/deploy/.ssh/authorized_keys" do
  path "/home/deploy/.ssh/authorized_keys"
  remote_path "Keys/#{node['rails_env']}/authorized_keys"
  bucket "echecks-infra-#{node['vpc_env']}"
  owner "deploy"
  group "deploy"
  mode '0400'
  action :create
end

# bash 'add deploy key' do
#   live_stream true
#   user "deploy"
#   code <<-EOH
#       eval `ssh-agent`
#       ssh-add /home/deploy/.ssh/id_github_key
#       EOH
# end

# utils = node['custom-infra-utils']
#
# ruby_block 'load_data_bags_for_github_key' do
#   block do
#     node.run_state['github_key'] = Chef::EncryptedDataBagItem.load('github', 'id_deploy')
#   end
#   action :run
# end
#
# file utils['key_path'] do
#   content lazy { node.run_state['github_key']['private_key'] }
#   sensitive true
#   owner utils['user']
#   group utils['group']
#   mode '0600'
#   action :create
# end
#
# cookbook_file "#{Chef::Config[:file_cache_path]}wrap-ssh4git.sh" do
#   source 'wrap-ssh4git.sh'
#   owner utils['user']
#   mode '0755'
# end
#
# include_recipe 'custom-infra-utils::deploy'

#
# execute 'git' do
#   command 'echo configure git'
# end
