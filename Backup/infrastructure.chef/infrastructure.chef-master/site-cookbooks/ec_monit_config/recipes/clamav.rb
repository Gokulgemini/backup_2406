template '/etc/monit.d/clamav.monitrc' do
  mode 0644
  source 'clamav.monitrc.erb'
  action :create
end

restart_monit_process 'clamav' do
  process_name 'clamav'
end
