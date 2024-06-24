pen_databag = "#{node['vpc_env']}_users"

users_manage 'penadmins' do
  group_id 3000
  action [:create]
  data_bag pen_databag
end

sudo 'penadmins' do
  groups                 "penadmins"
  nopasswd               true
end
