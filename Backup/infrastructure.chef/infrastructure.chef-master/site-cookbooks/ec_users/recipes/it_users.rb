##
#
# IT needs us to set these initial passwords until CyberArk is in Place
##
admin_databag = "#{node['vpc_env']}_users"

# User for CCASCANNER
users_manage 'ccascan' do
  group_id 1005
  action [:create]
  data_bag admin_databag
  only_if { `cat /etc/shadow | grep ccascan | cut -d : -f 2`.strip!.eql? '!' }
end

# User for itimadm
users_manage 'itimadm' do
  group_id 1001
  action [:create]
  data_bag admin_databag
  only_if { `cat /etc/shadow | grep itimadm | cut -d : -f 2`.strip!.eql? '!' }
end
