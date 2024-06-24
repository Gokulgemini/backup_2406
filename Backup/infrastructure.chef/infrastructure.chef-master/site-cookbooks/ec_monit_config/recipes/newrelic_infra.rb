template "/etc/monit.d/newrelic_infra.monitrc" do
  source "newrelic_infra.monitrc.erb"
  owner "root"
  group "root"
  mode "0644"
  backup false
  notifies :run, 'execute[monit reload]', :delayed
end

execute "monit reload" do
  action :nothing
  notifies :run, 'ruby_block[wait_for_newrelic_load]', :delayed
end

execute "monit restart newrelic_infra" do
  action :nothing
end

ruby_block 'wait_for_newrelic_load' do
  block do
    sleep 20
  end
  action :nothing
  notifies :run, 'execute[monit restart newrelic_infra]', :delayed
end
