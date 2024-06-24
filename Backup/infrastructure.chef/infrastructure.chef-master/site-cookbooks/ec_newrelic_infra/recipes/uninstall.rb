# Cookbook Name:: custom-newrelic_infra
# Recipe:: uninstall
# Author:: David Rodriguez
#
# Uninstall the newrelic infra client

service 'newrelic-infra' do
  action :stop
  only_if "test -f /etc/init.d/newrelic-infra"
end

%w("/etc/newrelic-infra" "/usr/share/doc/newrelic-infra" "/var/db/newrelic-infra").each do |dir|
  # Directory resource isn't deleting so switching to execute_block
  execute "remove #{dir}" do
    command "rm -rf #{dir}"
    only_if "test -d #{dir}"
  end
end

%w("/etc/init.d/newrelic-infra" "/usr/bin/newrelic-infra" "/etc/newrelic-infra.yml" "/etc/monit.d/newrelic_infra.monitrc").each do |remove_file|
  # File resource isn't deleting so switching to execute_block
  execute "remove #{remove_file}" do
    command "rm -f #{remove_file}"
    only_if "test -f #{remove_file}"
  end
end

execute "monit reload" do
  action :run
end
