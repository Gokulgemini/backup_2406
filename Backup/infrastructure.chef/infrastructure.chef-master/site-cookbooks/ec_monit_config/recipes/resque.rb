rails_env = node['rails_env'].nil? ? node['ec_monit_config']['resque']['framework_env'] : node['rails_env']

retrievals_servers = search(:node, "roles:app_retrievals AND chef_environment:#{node['chef_environment']}" )

template "/etc/monit.d/resque.monitrc" do
  source "resque.monitrc.erb"
  owner "root"
  group "root"
  mode 0600
  action :create
  variables(
    :app_name => 'verifyvalid',
    :worker_count => node['ec_monit_config']['resque']['worker_count'],
    :rails_env => rails_env,
    :retrievals_exists => retrievals_servers
  )
  notifies :run, 'execute[monit reload]', :immediately
end

execute "monit reload" do
  action :nothing
  notifies :run, 'ruby_block[wait_for_resque_load]', :delayed
end

ruby_block 'wait_for_resque_load' do
  block do
    sleep 20
  end
  action :nothing
  notifies :run, 'execute[monit restart -g resque]', :delayed
end

execute "monit restart -g resque" do
  action :nothing
end
