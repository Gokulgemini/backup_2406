netowl = node['netowl']

template '/etc/monit.d/netowl-matcher.monitrc' do
  source 'netowl-matcher.monitrc.erb'
  owner 'root'
  mode '0755'
  variables(
    :pid => '/var/run/netowl/namematcher.pid',
    :startup => '/etc/init.d/matcher start',
    :shutdown => '/etc/init.d/matcher stop'
  )
  notifies :run, 'execute[reload_monit]', :immediately
end

execute 'reload_monit' do
  user 'root'
  command 'monit reload'
end

execute 'start_netowl' do
  action :run
  user 'root'
  command 'monit start netowl-matcher'
  not_if { `monit status netowl-matcher | grep status | grep OK` }
end
