service 'nginx' do
  action :nothing
end

s3_key_prefix = "#{node['ec_nginx']['s3_artifact_folder']}"
bucket_name = "echecks-infra-#{node['vpc_env']}"

s3_file "Download vv cert key" do
  path "/etc/ssl/verifyvalid-production-cert-pri.key"
  remote_path "ssl-certs/verifyvalid-production-cert-pri.key"
  bucket bucket_name
  owner 'deploy'
  group 'deploy'
  mode '0600'
  action :create
end

s3_file "Download vv cert" do
  path "/etc/ssl/verifyvalid-production-cert.cer"
  remote_path "ssl-certs/verifyvalid-production-cert.cer"
  bucket 'echecks-infra-prod'
  owner 'deploy'
  group 'deploy'
  action :create
end


key_file = 'verifyvalid-production-cert-pri.key'
cert_file = 'verifyvalid-production-cert.cer'

execute 'create nginx servers directory' do
  action :run
  not_if { ::File.directory?("/etc/nginx/servers") }
  command 'mkdir /etc/nginx/servers'
end

template "/etc/nginx/servers/api_verifyvalid_com.conf" do
  owner node['ec_nginx']['user']
  group node['ec_nginx']['user']
  mode 0644
  source "verifyvalid.nginx.conf.erb"
  variables(
    app_name:        'api_verifyvalid',
    server_names:    'api.verifyvalid.com .verifyvalid.com',
    server_port:     '19009',
    ssl_key:         "/etc/ssl/#{key_file}",
    ssl_cert:        "/etc/ssl/#{cert_file}"
  )
  notifies :reload, 'service[nginx]', :delayed
end

template "/etc/nginx/servers/catch_all_verifyvalid_com.conf" do
  owner node['ec_nginx']['user']
  group node['ec_nginx']['user']
  mode 0644
  source "catchall.nginx.conf.erb"
  variables(
    server_port:     '19010',
    ssl_key:         "/etc/ssl/#{key_file}",
    ssl_cert:        "/etc/ssl/#{cert_file}"
  )
  notifies :reload, 'service[nginx]', :delayed
end
