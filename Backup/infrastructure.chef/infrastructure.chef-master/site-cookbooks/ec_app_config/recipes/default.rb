## #
## # Cookbook Name:: app_config
## # Recipe:: default
## #
##
bucket_name = "echecks-app-config-#{node['vpc_env']}"
apps = node['ec_app_config']['apps']

directory "/data" do
  owner 'deploy'
  group 'deploy'
  mode '0755'
  action :create
  recursive true
end

apps.each do |app_name|
  directory "/data/#{app_name}" do
    owner 'deploy'
    group 'deploy'
    mode '0755'
    action :create
    recursive true
  end

  directory "/data/#{app_name}/shared" do
    owner 'deploy'
    group 'deploy'
    mode '0755'
    action :create
    recursive true
  end

  directory "/data/#{app_name}/shared/config" do
    owner 'deploy'
    group 'deploy'
    mode '0755'
    action :create
    recursive true
  end

  if ['verifyvalid', 'monitoring'].include?(app_name.downcase)
    if node['roles'].include?('delayed_job_server')

      file '/etc/cron.allow' do
        action :delete
      end

      execute "bundle_install_#{app_name}" do
        user "deploy"
        cwd "/data/#{app_name}/current"
        command "bundle install"
        only_if { File.exists?("/data/#{app_name}/current") && `bundle check`  }
      end

      execute "add_whenever_cronjobs_#{app_name}" do
        user "deploy"
        cwd "/data/#{app_name}/current"
        command "bundle exec whenever --set environment=#{node['rails_env']} --update-crontab #{app_name}_#{node['rails_env']}"
        only_if { File.exists?("/data/#{app_name}/current") && node['roles'].include?('delayed_job_server') }
      end

      file '/etc/cron.allow' do
        action :create
      end

      cron_access 'allow cron for deploy' do
        user 'deploy'
        action :allow
      end

      execute 'fix_allow_permissions' do
        command 'chmod 644 /etc/cron.allow'
      end
    end
  end

  config_files = [
    {
      shared: true,
      filepath: "/data/#{app_name}/shared/config",
      remote_filename: 'quickbooks_online.yml',
      apps: ['verifyvalid', 'console', 'retrievals']
    },
    {
      shared: true,
      filepath: "/data/#{app_name}/shared/config",
      remote_filename: 'yodlee.yml',
      apps: ['verifyvalid', 'console', 'retrievals']
    },
    {
      shared: true,
      filepath: "/data/#{app_name}/shared/config",
      remote_filename: 'ingo.yml',
      apps: ['verifyvalid', 'console', 'retrievals']
    },
    {
      shared: true,
      filepath: "/data/#{app_name}/shared/config",
      remote_filename: 'smtp_settings.yml',
      apps: ['verifyvalid', 'console', 'monitoring', 'registrar', 'retrievals']
    },
    {
      shared: true,
      filepath: "/data/#{app_name}/shared/config",
      remote_filename: 'microbilt.yml',
      apps: ['verifyvalid', 'console', 'retrievals']
    },
    {
      shared: true,
      filepath: "/data/#{app_name}/shared/config",
      remote_filename: 'keys.yml',
      apps: ['verifyvalid', 'retrievals']
    },
    {
      shared: true,
      filepath: "/data/#{app_name}/shared/config",
      remote_filename: 'api.yml',
      apps: ['registrar', 'onboard', 'monitoring']
    },
    {
      shared: true,
      filepath: "/data/#{app_name}/shared/config",
      remote_filename: 'verifyvalid.yml',
      apps: ['donations']
    },
    {
      shared: true,
      filepath: "/data/#{app_name}/shared/config",
      remote_filename: 'maxmind.yml',
      apps: ['verifyvalid', 'console', 'retrievals']
    },
    {
      shared: true,
      filepath: "/data/#{app_name}/shared/config",
      remote_filename: 'secrets.yml',
      apps: ['verifyvalid', 'console', 'retrievals']
    },
    {
      shared: true,
      filepath: "/data/#{app_name}/shared/config",
      remote_filename: 'hyperwallet.yml',
      apps: ['verifyvalid', 'console', 'retrievals']
    },
    {
      shared: true,
      filepath: "/data/#{app_name}/shared/config",
      remote_filename: 'valid_systems.yml',
      apps: ['verifyvalid', 'console', 'retrievals']
    },
    {
      shared: true,
      filepath: "/data/#{app_name}/shared/config",
      remote_filename: 'newrelic.yml',
      apps: ['verifyvalid']
    },
    {
      shared: true,
      filepath: "/data/#{app_name}/shared/config",
      remote_filename: 'newrelic_console.yml',
      filename: 'newrelic.yml',
      apps: ['console']
    },
    {
      shared: true,
      filepath: "/data/#{app_name}/shared/config",
      remote_filename: 'zendesk.yml',
      apps: ['verifyvalid', 'console', 'retrievals']
    },
    {
      shared: true,
      filepath: "/data/#{app_name}/shared/config",
      remote_filename: 'daily_transaction_report.yml',
      apps: ['verifyvalid', 'console', 'retrievals']
    },
    {
      shared: true,
      filepath: "/data/#{app_name}/shared/config",
      remote_filename: 'twilio.yml',
      apps: ['verifyvalid','console', 'retrievals']
    },{
      shared: true,
      filepath: "/data/#{app_name}/shared/config",
      remote_filename: 'symmetric-encryption.yml',
      apps: ['verifyvalid', 'console', 'registrar', 'retrievals']
    },{
      shared: true,
      filepath: "/data/#{app_name}/shared/config",
      remote_filename: 'echo.yml',
      apps: ['verifyvalid', 'console', 'retrievals']
    },
    {
      shared: false,
      filepath: "/data/#{app_name}/shared/config",
      remote_filename: 'net_owl.yml',
      apps: ['verifyvalid', 'console', 'retrievals']
    },
    {
      shared: true,
      filepath: "/data/#{app_name}/shared/config",
      remote_filename: 'usps_web_tools_api.yml',
      apps: ['verifyvalid', 'console', 'retrievals']
    },
    {
      shared: false,
      filepath: "/data/#{app_name}/shared/config",
      remote_filename: 'aws.yml',
      apps: ['verifyvalid', 'console', 'retrievals']
    },
    {
      shared: false,
      filepath: "/data/#{app_name}/shared/config",
      remote_filename: 'aws-registrar.yml',
      filename: 'aws.yml',
      apps: ['registrar']
    },
    {
      shared: false,
      filepath: "/data/#{app_name}/shared/config",
      remote_filename: 'elasticsearch.yml',
      filename: 'elasticsearch.yml',
      apps: ['verifyvalid', 'console', 'retrievals']
    },
    {
      shared: true,
      filepath: "/data/#{app_name}/shared/config",
      remote_filename: "system.yml",
      filename: 'system.yml',
      apps: ['registrar']
    },
    {
      shared: true,
      filepath: "/data/#{app_name}/shared/config",
      remote_filename: "cmp.yml",
      filename: 'cmp.yml',
      apps: ['VerifyValid', 'verifyvalid', 'console', 'retrievals']
    },
    {
      shared: true,
      filepath: "/data/#{app_name}/shared/config",
      remote_filename: "email_service.yml",
      filename: 'email_service.yml',
      apps: ['VerifyValid', 'verifyvalid', 'console', 'retrievals']
    },
    {
      shared: true,
      filepath: "/data/#{app_name}/shared/config",
      remote_filename: "stream.yml",
      filename: 'stream.yml',
      apps: ['VerifyValid', 'verifyvalid', 'console', 'retrievals']
    },
    {
    shared: true,
    filepath: "/data/#{app_name}/shared/config",
    remote_filename: 'newrelic_retrievals.yml',
    filename: 'newrelic.yml',
    apps: ['retrievals']
    },
  ]

  # only run the config needed for the app
  # config_files is modified between runs, so re-define it
  configs = config_files.keep_if { |config| config[:apps].include?(app_name) }
  next if configs.empty?
  configs.each do |config_values|

    download_location = config_values[:shared] ? 'shared' : node['rails_env']
    remote_filename = config_values[:remote_filename]
    config_file = config_values[:filename] || remote_filename

    s3_file "Download #{app_name} #{remote_filename} from s3" do
      path "/data/#{app_name}/shared/config/#{config_file}"
      remote_path "#{download_location}/#{remote_filename}"
      bucket bucket_name
      owner 'deploy'
      group 'deploy'
      mode '0744'
      action :create
      only_if { Dir.exists?("/data/#{app_name}/shared/config") }
    end
  end
end

include_recipe 'ec_app_config::generate_configs'
