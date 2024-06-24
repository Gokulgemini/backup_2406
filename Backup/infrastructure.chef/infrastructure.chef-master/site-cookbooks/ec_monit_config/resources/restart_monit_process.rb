# This resource will do the appropriate reloading and restarting
# of the defined monit process.   One thing to add in the future
# is the ability to deal with monit groups.
# Going forward we should use this resource and slowly convert existing
# processes to it.

resource_name :restart_monit_process

property :process_name, String, name_property: true

action :run do
  execute "monit reload" do
    action :run
    notifies :run, 'ruby_block[wait_for_monit_reload]', :delayed
  end

  ruby_block 'wait_for_monit_reload' do
    block do
      sleep 20
    end
    action :nothing
    notifies :run, "execute[monit_restart_#{new_resource.process_name}]", :delayed
  end

  execute "monit_restart_#{new_resource.process_name}" do
    command 'monit restart clamav'
    action :nothing
  end
end
