file '/etc/cron.allow' do
  action :delete
end

cronheaders = <<~END
  PATH=/usr/local/bin:/usr/bin:/bin
  RAILS_ENV="#{node['rails_env']}"
  MAILTO=""
END

file '/home/deploy/cronheaders' do
  content cronheaders
  only_if do File.exist?('/home/deploy') end
end

#todo add another idempotency chef incase /home/deploy/cronheaders is accidentally deleted
execute 'load cron headers' do
  command 'crontab -u deploy /home/deploy/cronheaders'
  only_if { File.exists?('/home/deploy/cronheaders') }
end
