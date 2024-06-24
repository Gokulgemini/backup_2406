apps = node['apps'].nil? ? node['ec_monit_config']['unicorn']['app_names'] : node['apps']

apps.each_pair do |config,app|

app_name = app[:name]

worker_memory_size = 600
worker_termination_conditions = {'quit' => [{}], 'term' => [{'cycles' => 8}]}

base_cycles = (worker_termination_conditions.fetch('quit',[]).detect {|h| h.key?('cycles')} || {}).fetch('cycles',2).to_i

worker_mem_cycle_checks = []
%w(quit abrt term kill).each do |sig|
  worker_termination_conditions.fetch(sig,[]).each do |condition|
    overrun_cycles = condition.fetch('cycles',base_cycles).to_i
    mem = condition.fetch('memory',worker_memory_size).to_f
    worker_mem_cycle_checks << [mem, overrun_cycles, sig]
  end
end

template "/etc/monit.d/unicorn_#{app_name}.monitrc" do
  owner "root"
  group "root"
  mode 0600
  source "unicorn.monitrc.erb"
  variables(
    lazy {
      {
        :app => app_name,
        :user => "deploy",
        :unicorn_worker_count => node['ec_unicorn'][app_name].nil? ? 2 : node['ec_unicorn'][app_name]['number_of_workers'].to_i,
        :environment => "development",
        :master_memory_size => worker_memory_size,
        :master_cycle_count => base_cycles,
        :worker_mem_cycle_checks => worker_mem_cycle_checks
      }
    }
  )
  backup 0

  notifies :run, 'execute[reload_monit]', :immediately
end

# cleanup extra unicorn workers
# bash "cleanup extra unicorn workers" do
#   code lazy {
#     <<-EOH
#       for pidfile in /data/temp/#{app[:name]}/shared/pids/unicorn_worker_#{app[:name]}_*.pid; do
#         [[ $(echo "${pidfile}" | egrep -o '([0-9]+)' | tail -n 1) -gt #{recipe.get_pool_size - 1} ]] && kill -QUIT $(cat $pidfile) || true
#       done
#     EOH
#   }
# end


execute 'reload_monit' do
  user 'root'
  command 'monit reload'
  action :nothing
  notifies :run, "execute[monit start unicorn_master_#{app_name}]", :delayed
end

# ruby_block 'wait_for_reload' do
#   block do
#     sleep 5
#   end
#   action :nothing
# end

execute "monit start unicorn_master_#{app_name}" do
  action :nothing
  user 'root'
  command "monit start unicorn_master_#{app_name}"
  not_if { `monit status unicorn_master_#{app_name} | grep status | grep Running` }
end

end
