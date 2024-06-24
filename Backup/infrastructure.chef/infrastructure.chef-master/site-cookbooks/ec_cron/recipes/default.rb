include_recipe 'ec_cron::initialize'

crons = node['ec_cron']['echeck_crons']
crons.each do |cron|
  cron cron[:name] do
    user     'deploy'
    action   :create
    minute   cron[:time].split[0]
    hour     cron[:time].split[1]
    day      cron[:time].split[2]
    month    cron[:time].split[3]
    weekday  cron[:time].split[4]
    command  cron[:command]
  end
end

file '/etc/cron.allow' do
  action :create
end

cron_access 'allow cron for deploy' do
  user 'deploy'
  action :nothing
  subscribes :allow, 'file[/etc/cron.allow]', :immediately
end

execute 'fix_allow_permissions' do
  command 'chmod 644 /etc/cron.allow'
  action :nothing
  subscribes :run, 'file[/etc/cron.allow]', :delayed
end

file '/etc/rsyslog.d/cron.conf' do
  content 'cron.* /var/log/cron.log'
  notifies :run, 'execute[restart_syslog]', :delayed
end

ec_cron_log_cleanup = node['ec_cron']['ec_cron_log']
file ec_cron_log_cleanup do
  owner  'deploy'
  group  'deploy'
  mode   '644'
  action :create
  not_if { ::File.exist?("#{ec_cron_log_cleanup}") }
end

ec_cleanup_log_conf = node['ec_cron']['ec_cleanup_conf']
file ec_cleanup_log_conf do
  content "if $programname startswith 'CRON' and $msg contains 'deploy' then #{ec_cleanup_log_conf}"
  action :create
  notifies :run, 'execute[restart_syslog]', :delayed 
  not_if { ::File.exist?("#{ec_cleanup_log_conf}") }
end

execute 'restart_syslog' do
  command 'systemctl restart rsyslog'
  action :nothing
end
