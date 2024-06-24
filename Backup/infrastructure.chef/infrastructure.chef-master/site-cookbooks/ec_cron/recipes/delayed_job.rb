include_recipe 'ec_cron::default'

file '/etc/cron.allow' do
  action :delete
end

crons = node['ec_cron']['delayed_job_crons']
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
