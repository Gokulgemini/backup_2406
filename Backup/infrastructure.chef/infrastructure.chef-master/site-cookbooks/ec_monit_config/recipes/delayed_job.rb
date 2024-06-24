rails_env = node['rails_env'].nil? ? node['ec_monit_config']['delayed_job']['framework_env'] : node['rails_env']

retrievals_servers = search(:node, "roles:app_retrievals AND chef_environment:#{node['chef_environment']}" )

# Some per app/env config
echecks_path = node['ec_monit_config']['delayed_job']['echecks_path']

# The next two lines have to match with the application
echecks_delayed_job_worker_count = node['ec_monit_config']['delayed_job']['worker_count']
echecks_delayed_job_options = node['ec_monit_config']['delayed_job']['job_options']

#registrar_path = default['ec_monit_config']['delayed_job']['registrar_path']


#TODO fix template naming to match process name not urgent
template "/etc/monit.d/delayed_jobs_verifyvalid.monitrc" do
  owner "root"
  group "root"
  mode 0600
  source 'delayed_jobs.monitrc.erb'
  action :create
  variables(
    :app_name => 'verifyvalid',
    :app_path => echecks_path,
    :delayed_job_worker_count => echecks_delayed_job_worker_count,
    :delayed_job_options => echecks_delayed_job_options,
    :delayed_job_start => "/bin/delayed_job",
    :rails_env => rails_env,
    :retrievals_exists => retrievals_servers
  )
  notifies :run, 'execute[reload_monit]', :delayed
end

#TODO: fix these using templates correctly...
template "/etc/monit.d/delayed_jobs_registrar.monitrc" do
  owner "root"
  group "root"
  mode 0600
  source 'registrar_delayed_job.monitrc.erb'
  action :create
  variables(
    :app_name => 'registrar',
    :app_path => "/data/registrar/current",
    #:echecks_delayed_job_worker_count => 2,
    #:echecks_delayed_job_options => "",
    :delayed_job_start => "/bin/delayed_job",
    :rails_env => rails_env
  )
  notifies :run, 'execute[reload_monit]', :delayed
end

# template "/etc/monit.d/delayed_jobs_monitoring.monitrc" do
#   owner "root"
#   group "root"
#   mode 0600
#   source 'delayed_jobs.monitrc.erb'
#   action :create
#   variables(
#     :app_name => 'monitoring',
#     :echecks_path => "/data/monitoring/current",
#     :echecks_delayed_job_worker_count => 2,
#     :echecks_delayed_job_options => "",
#     :delayed_job_start => "script/delayed_job",
#     :rails_env => rails_env
#   )
#   notifies :run, 'execute[reload_monit]', :immediately
# end

#TODO: fix these using templates correctly...
template "/etc/monit.d/delayed_jobs_onboard.monitrc" do
  owner "root"
  group "root"
  mode 0600
  source 'onboard_delayed_job.monitrc.erb'
  action :create
  variables(
    :app_name => 'onboard',
    :echecks_path => "/data/onboard/current",
    #:delayed_job_worker_count => 2,
    #:delayed_job_options => "",
    :delayed_job_start => "/script/delayed_job",
    :rails_env => rails_env
  )
  notifies :run, 'execute[reload_monit]', :delayed
end

# cleanup extra unicorn workers
# bash "cleanup extra unicorn workers" do
#   code lazy {
#     <<-EOH
#       for pidfile in /data/temp/#{app_name}/shared/pids/unicorn_worker_#{app_name}_*.pid; do
#         [[ $(echo "${pidfile}" | egrep -o '([0-9]+)' | tail -n 1) -gt #{recipe.get_pool_size - 1} ]] && kill -QUIT $(cat $pidfile) || true
#       done
#     EOH
#   }
# end


execute 'reload_monit' do
  user 'root'
  command 'monit reload'
  action :nothing
  #notifies :run, 'ruby_block[wait_for_reload]', :immediately
end

# ruby_block 'wait_for_reload' do
#   block do
#     sleep 5
#   end
#   action :nothing
# end

# execute 'start_unicorn_monit' do
#   user 'root'
#   command "monit start unicorn_master_#{app_name}"
#   not_if { `monit status unicorn_master_#{app_name} | grep status | grep Running` }
# end


# TODO: convert to partial templates
# template "/etc/monit.d/delayed_jobs.monitrc" do
#   owner "root"
#   group "root"
#   mode 0600
#   source 'delayed_jobs.monitrc.erb'
#   action :create
#   variables(
#     :echecks_path => echecks_path,
#     :echecks_delayed_job_worker_count => echecks_delayed_job_worker_count,
#     :echecks_delayed_job_options => echecks_delayed_job_options,
#     :registrar_path => registrar_path,
#     :rails_env => railes_env,
#   )
# end
#
# template "/etc/monit.d/delayed_jobs.monitrc" do
#   owner "root"
#   group "root"
#   mode 0600
#   source 'delayed_jobs.monitrc.erb'
#   action :create
#   variables(
#     :echecks_path => echecks_path,
#     :echecks_delayed_job_worker_count => echecks_delayed_job_worker_count,
#     :echecks_delayed_job_options => echecks_delayed_job_options,
#     :registrar_path => registrar_path,
#     :rails_env => railes_env,
#   )
# end
