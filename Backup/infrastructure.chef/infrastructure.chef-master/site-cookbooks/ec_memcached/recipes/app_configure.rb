memcached_instances = search(:node, "server_role:app AND chef_environment:#{node['chef_environment']}", :filter_result => {
  'ip': ['ipaddress']
}).map{|instance| instance['ip']}

apps = node['ec_app_config']['apps']

if ((node['server_role'].eql? 'app') || (node['server_role'].eql? 'delayed_job') || (node['server_role'].eql? 'retrievals') || (node['server_role'].eql? 'resque'))
  apps.each do |app_name|
    template "/data/#{app_name}/shared/config/memcached.yml" do
      source "memcached.yml.erb"
      owner 'deploy'
      group 'deploy'
      mode 0744
      variables({
        :rails_env => node['rails_env'],
        :server_names => memcached_instances
      })
      only_if { Dir.exists?("/data/#{app_name}/shared/config") }
    end
  end
end
