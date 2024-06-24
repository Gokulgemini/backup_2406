## #
## # Cookbook Name:: ec_app_config
## # Recipe:: database
## #
##

chef_env = node['chef_environment']
app_configs = data_bag_item(chef_env, 'app_configs')

app_configs[:apps].each do |app|
  app['configs'].each do |config|
    config.each do |config_file, config|
      template "/data/#{app['name']}/shared/config/#{config_file}" do
        source "#{config_file}.erb"
        owner 'deploy'
        group 'deploy'
        mode '0755'
        variables({
          :configuration => lazy { config }
        })
        action :create
        only_if { Dir.exists?("/data/#{app['name']}/shared/config") }
      end
    end
  end
end
