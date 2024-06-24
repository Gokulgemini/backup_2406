deploy_databag = "#{node['vpc_env']}_users"

users_manage 'deploy' do
  group_id 1006
  action [:create]
  data_bag deploy_databag
end

sudo 'deploy' do
  user 'deploy'
  nopasswd true
end
