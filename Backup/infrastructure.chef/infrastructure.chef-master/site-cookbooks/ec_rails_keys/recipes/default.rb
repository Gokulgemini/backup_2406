
# Copy the encryption keys from S3
directory '/data/rails/keys' do
  mode 0770
  recursive true
  owner 'deploy'
  group 'deploy'
  action :create
end

#s3_key_prefix = "#{node['rails_env']}/#{app_name}/"
s3_key_prefix = "shared/keys/"
bucket_name = "echecks-app-config-#{node['vpc_env']}"

if node.chef_environment != "vagrant"
  node['ec_rails_keys']['s3_files'].each do |encryption_file|
    s3_file "Download #{encryption_file} from s3" do
      path "/data/rails/keys/#{encryption_file}"
      remote_path s3_key_prefix + encryption_file
      bucket bucket_name
      owner node['ec_rails_keys']['owner']
      group node['ec_rails_keys']['group']
      mode '0440'
      owner 'deploy'
      group 'deploy'
      action :create
    end
  end
end
