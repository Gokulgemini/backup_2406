apps = node['apps'].nil? ? node['ec_unicorn']['app_name'] : node['apps']

apps.each_pair do |config,app|
  app_name = app[:name]
  configs = "/data/#{app_name}/shared/config"
  number_of_workers = node['ec_unicorn'][app_name].nil? ? 2 : node['ec_unicorn'][app_name]['number_of_workers'].to_i
  unicorn_socket = "/data/#{app_name}/shared/sockets/unicorn_#{app_name}.sock"
  pid_file = "/data/#{app_name}/shared/pids/unicorn_#{app_name}.pid"
  semantic_logger = node['ec_unicorn'][app_name].nil? ? false  : node['ec_unicorn'][app_name]['semantic_logger']
  template "#{configs}/unicorn.rb" do
    mode 0644
    source 'unicorn.custom.erb'
    variables(
      number_of_workers:  number_of_workers,
      unicorn_socket:     unicorn_socket,
      pid_file:           pid_file,
      semantic_logger:    semantic_logger
    )
    only_if {  Dir.exists?(configs) }
  end
end
