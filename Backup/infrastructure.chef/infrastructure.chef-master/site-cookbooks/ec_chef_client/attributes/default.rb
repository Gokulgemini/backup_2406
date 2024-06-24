

default['chef_client']['systemd']['killmode'] = 'process'

default['chef_client']['config'] = {
  'chef_server_url' => Chef::Config[:chef_server_url],
  'validation_client_name' => Chef::Config[:validation_client_name],
  'node_name' => Chef::Config[:node_name],
  'verify_api_cert' => true,
}

# Accept the chef license when running the chef service
default['chef_client']['chef_license'] = nil

# should the client fork on runs
default['chef_client']['config']['client_fork'] = true

default['chef_client']['log_file']    = 'client.log'
default['chef_client']['interval']    = '1800'
default['chef_client']['splay']       = '300'
default['chef_client']['conf_dir']    = '/etc/chef'
default['chef_client']['bin']         = '/usr/bin/chef-client'

# Set a sane default log directory location, overriden by specific
# platforms below.
default['chef_client']['log_dir']     = '/var/log/chef'

# If log file is used, default permissions so everyone can read
default['chef_client']['log_perm'] = '640'

# Configuration for chef-client::cron recipe.
default['chef_client']['cron'] = {
  'minute' => '0',
  'hour' => '0,4,8,12,16,20',
  'weekday' => '*',
  'path' => nil,
  'environment_variables' => nil,
  'log_file' => '/dev/null',
  'append_log' => false,
  'use_cron_d' => false,
  'mailto' => nil,
}

# Configuration for chef-client::systemd_service recipe
default['chef_client']['systemd']['timer'] = false
# Systemd timeout. Might be useful for timer setups to avoid stalled chef runs
default['chef_client']['systemd']['timeout'] = false
# Restart mode when not running as a timer
default['chef_client']['systemd']['restart'] = 'always'

# Configuration for Windows scheduled task
default['chef_client']['task']['frequency'] = 'minute'
default['chef_client']['task']['frequency_modifier'] = node['chef_client']['interval'].to_i / 60
default['chef_client']['task']['user'] = 'SYSTEM'
default['chef_client']['task']['password'] = nil # Password is only required for non-system users
default['chef_client']['task']['start_time'] = nil
default['chef_client']['task']['start_date'] = nil
default['chef_client']['task']['name'] = 'chef-client'

default['chef_client']['load_gems'] = {}

default['chef_client']['config']['start_handlers'] = []
default['chef_client']['config']['report_handlers'] = []
default['chef_client']['config']['exception_handlers'] = []

# If set to false, changes in the `client.rb` template won't trigger a reload
# of those configs in the current Chef run.
default['chef_client']['reload_config'] = true

# Any additional daemon options can be set as an array. This will be
# join'ed in the relevant service configuration.
default['chef_client']['daemon_options'] = []

# Ohai plugins to be disabled are configured in /etc/chef/client.rb,
# so they can be set as an array in this attribute.
default['ohai']['disabled_plugins'] = []

# An additional path to load Ohai plugins from.
default['ohai']['plugin_path'] = nil

# Use logrotate_app definition on supported platforms via config recipe
# when chef_client['log_file'] is set.
# Default rotate: 12; frequency: weekly
default['chef_client']['logrotate']['rotate'] = 12
default['chef_client']['logrotate']['frequency'] = 'weekly'

case node['platform_family']
when 'aix'
  default['chef_client']['init_style']  = 'src'
  default['chef_client']['svc_name']    = 'chef'
  default['chef_client']['run_path']    = '/var/run/chef'
  default['chef_client']['cache_path']  = '/var/spool/chef'
  default['chef_client']['backup_path'] = '/var/lib/chef'
  default['chef_client']['log_dir']     = '/var/adm/chef'
when 'amazon', 'rhel', 'fedora', 'debian', 'suse', 'clearlinux'
  default['chef_client']['init_style']  = node['init_package']
  default['chef_client']['run_path']    = '/var/run/chef'
  default['chef_client']['cache_path']  = '/var/cache/chef'
  default['chef_client']['backup_path'] = '/var/lib/chef'
  default['chef_client']['chkconfig']['start_order'] = 98
  default['chef_client']['chkconfig']['stop_order']  = 02
when 'freebsd'
  default['chef_client']['init_style']  = 'bsd'
  default['chef_client']['run_path']    = '/var/run'
  default['chef_client']['cache_path']  = '/var/chef/cache'
  default['chef_client']['backup_path'] = '/var/chef/backup'
# don't use bsd paths per COOK-1379
when 'mac_os_x'
  default['chef_client']['init_style']  = 'launchd'
  default['chef_client']['log_dir']     = '/Library/Logs/Chef'
  # Launchd doesn't use pid files
  default['chef_client']['run_path']    = '/var/run/chef'
  default['chef_client']['cache_path']  = '/Library/Caches/Chef'
  default['chef_client']['backup_path'] = '/Library/Caches/Chef/Backup'
  # Set to 'daemon' if you want chef-client to run
  # continuously with the -d and -s options, or leave
  # as 'interval' if you want chef-client to be run
  # periodically by launchd
  default['chef_client']['launchd_mode'] = 'interval'
when 'openindiana', 'opensolaris', 'nexentacore', 'solaris2', 'omnios'
  default['chef_client']['init_style']  = 'smf'
  default['chef_client']['run_path']    = '/var/run/chef'
  default['chef_client']['cache_path']  = '/var/chef/cache'
  default['chef_client']['backup_path'] = '/var/chef/backup'
  default['chef_client']['method_dir'] = '/lib/svc/method'
  default['chef_client']['bin_dir'] = '/usr/bin'
  default['chef_client']['locale'] = 'en_US.UTF-8'
  default['chef_client']['env_path'] = '/usr/local/sbin:/usr/local/bin:/sbin:/bin:/usr/sbin:/usr/bin'
when 'smartos'
  default['chef_client']['init_style']  = 'smf'
  default['chef_client']['run_path']    = '/var/run/chef'
  default['chef_client']['cache_path']  = '/var/chef/cache'
  default['chef_client']['backup_path'] = '/var/chef/backup'
  default['chef_client']['method_dir'] = '/opt/local/lib/svc/method'
  default['chef_client']['bin_dir'] = '/opt/local/bin'
  default['chef_client']['locale'] = 'en_US.UTF-8'
  default['chef_client']['env_path'] = '/usr/local/sbin:/usr/local/bin:/opt/local/sbin:/opt/local/bin:/usr/sbin:/usr/bin:/sbin'
when 'windows'
  default['chef_client']['init_style']  = 'windows'
  default['chef_client']['conf_dir']    = 'C:/chef'
  default['chef_client']['run_path']    = "#{node['chef_client']['conf_dir']}/run"
  default['chef_client']['cache_path']  = "#{node['chef_client']['conf_dir']}/cache"
  default['chef_client']['backup_path'] = "#{node['chef_client']['conf_dir']}/backup"
  default['chef_client']['log_dir']     = "#{node['chef_client']['conf_dir']}/log"
  default['chef_client']['bin']         = 'C:/opscode/chef/bin/chef-client'
else
  default['chef_client']['init_style']  = 'none'
  default['chef_client']['run_path']    = '/var/run'
  default['chef_client']['cache_path']  = '/var/chef/cache'
  default['chef_client']['backup_path'] = '/var/chef/backup'
end

# Must appear after init_style to take effect correctly
default['chef_client']['log_rotation']['options'] = ['compress']
default['chef_client']['log_rotation']['prerotate'] = nil
default['chef_client']['log_rotation']['postrotate'] =  case node['chef_client']['init_style']
                                                        when 'systemd'
                                                          'systemctl reload chef-client.service >/dev/null || :'
                                                        when 'upstart'
                                                          'initctl reload chef-client >/dev/null || :'
                                                        else
                                                          '/etc/init.d/chef-client reload >/dev/null || :'
                                                        end



  #
  # node['chef_client']['interval'] - Sets Chef::Config[:interval] via command-line option for number of seconds between chef-client daemon runs. Default 1800.
  # node['chef_client']['splay'] - Sets Chef::Config[:splay] via command-line option for a random amount of seconds to add to interval. Default 300.
  # node['chef_client']['log_file'] - Sets the file name used to store chef-client logs. Default "client.log".
  # node['chef_client']['log_dir'] - Sets directory used to store chef-client logs. Default "/var/log/chef".
  # node['chef_client']['log_rotation']['options'] - Set options to logrotation of chef-client log file. Default ['compress'].
  # node['chef_client']['log_rotation']['prerotate'] - Set prerotate action for chef-client logrotation. Default to nil.
  # node['chef_client']['log_rotation']['postrotate'] - Set postrotate action for chef-client logrotation. Default to chef-client service reload depending on init system.
  # node['chef_client']['conf_dir'] - Sets directory used via command-line option to a location where chef-client search for the client config file . Default "/etc/chef".
  # node['chef_client']['bin'] - Sets the full path to the chef-client binary. Mainly used to set a specific path if multiple versions of chef-client exist on a system or the bin has been installed in a non-sane path. Default "/usr/bin/chef-client".
  # node['chef_client']['ca_cert_path'] - Sets the full path to the PEM-encoded certificate trust store used by chef-client when daemonized. If not set, default values are used.
  # node['chef_client']['cron']['minute'] - The minute that chef-client will run as a cron task. See cron recipe
  # node['chef_client']['cron']['hour'] - The hour that chef-client will run as a cron task. See cron recipe
  # node['chef_client']['cron']['weekday'] - The weekday that chef-client will run as a cron task. See cron recipe
  # node['chef_client']['cron']['environment_variables'] - Environment variables to pass to chef-client's execution (e.g. SSL_CERT_FILE=/etc/ssl/certs/ca-certificates.crt chef-client)
  # node['chef_client']['cron']['log_file'] - Location to capture the log output of chef-client during the chef run.
  # node['chef_client']['cron']['append_log'] - Whether to append to the log. Default: false chef-client output.
  # node['chef_client']['cron']['use_cron_d'] - If true, use the cron_d resource. If false (default), use the cron resource built-in to Chef.
  # node['chef_client']['cron']['mailto'] - If set, MAILTO env variable is set for cron definition
  # node['chef_client']['cron']['priority'] - If set, defines the scheduling priority for the chef-client process. MUST be a value between -20 and 19. ONLY applies to *nix-style operating systems.
  # node['chef_client']['reload_config'] - If true, reload Chef config of current Chef run when client.rb template changes (defaults to true)
  # node['chef_client']['daemon_options'] - An array of additional options to pass to the chef-client service, empty by default, and must be an array if specified.
  # node['chef_client']['systemd']['timer'] - If true, uses systemd timer to run chef frequently instead of chef-client daemon mode (defaults to false). This only works on platforms where systemd is installed and used.
  # node['chef_client']['systemd']['timeout'] - If configured, sets the systemd timeout. This might be useful to avoid stalled chef runs in the systemd timer setup.
  # node['chef_client']['systemd']['restart'] - The string to use for systemd Restart= value when not running as a timer. Defaults to always. Other possible options: no, on-success, on-failure, on-abnormal, on-watchdog, on-abort.
  # node['chef_client']['systemd']['killmode'] - If configured, the string to use for the systemd KillMode= value. This determines how PIDs spawned by the chef-client process are handled when chef-client PID stops. Options: control-group, process, mixed, none. Systemd defaults to control-group when this is not specified. More information can be found on the systemd.kill man page.
  # node['chef_client']['task']['frequency'] - Frequency with which to run the chef-client scheduled task (e.g., 'hourly', 'daily', etc.) Default is 'minute'.
  # node['chef_client']['task']['frequency_modifier'] - Numeric value to go with the scheduled task frequency. Default is node['chef_client']['interval'].to_i / 60
  # node['chef_client']['task']['start_time'] - The start time for the task in HH:mm format (ex: 14:00). If the frequency is minute default start time will be Time.now plus the frequency_modifier number of minutes.
  # node['chef_client']['task']['start_date'] - The start date for the task in m:d:Y format (ex: 12/17/2017). nil by default and isn't necessary if you're running a regular interval.
  # node['chef_client']['task']['user'] - The user the scheduled task will run as, defaults to 'SYSTEM'.
  # node['chef_client']['task']['password'] - The password for the user the scheduled task will run as, defaults to nil because the default user, 'SYSTEM', does not need a password.
  # node['chef_client']['task']['name'] - The name of the scheduled task, defaults to chef-client.
  # The following attributes are set on a per-platform basis, see the attributes/default.rb file for default values.
  #
  # node['chef_client']['init_style'] - Sets up the client service based on the style of init system to use. Default is based on platform and falls back to 'none'. See service recipes.
  # node['chef_client']['run_path'] - Directory location where chef-client should write the PID file. Default based on platform, falls back to "/var/run".
  # node['chef_client']['cache_path'] - Directory location for
  # Chef::Config[:file_cache_path] where chef-client will cache various files. Default is based on platform, falls back to "/var/chef/cache".
  # node['chef_client']['backup_path'] - Directory location for Chef::Config[:file_backup_path] where chef-client will backup templates and cookbook files. Default is based on platform, falls back to "/var/chef/backup".
  # node['chef_client']['launchd_mode'] - (Only for Mac OS X) if set to 'daemon', runs chef-client with -d and -s options; defaults to 'interval'.
  # When chef_client['log_file'] is set and running on a logrotate supported platform (debian, rhel, fedora family), use the following attributes to tune log rotation.
  #
  # node['chef_client']['logrotate']['rotate'] - Number of rotated logs to keep on disk, default 12.
  # node['chef_client']['logrotate']['frequency'] - How often to rotate chef client logs, default weekly.
  # This cookbook makes use of attribute-driven configuration with this attribute. See USAGE for examples.
  #
  # node['chef_client']['config'] - A hash of Chef::Config keys and their values, rendered dynamically in /etc/chef/client.rb.
  # node['chef_client']['load_gems'] - Hash of gems to load into chef via the client.rb file
  # node['ohai']['disabled_plugins'] - An array of ohai plugins to disable, empty by default, and must be an array if specified. Ohai 6 plugins should be specified as a string (ie. "dmi"). Ohai 7+ plugins should be specified as a symbol within quotation marks (ie. ":Passwd").
  # node['ohai']['plugin_path'] - An additional path to load Ohai plugins from. Necessary if you use the ohai_plugin resource in the Ohai cookbook to install your own ohai plugins.
  # Chef Client Config
  # For the most current information about Chef Client configuration, read the documentation..
  #
  # node['chef_client']['chef_license'] - Set to 'accept' or 'accept-no-persist' to accept the license before upgrading to Chef 15.
  # node['chef_client']['config']['chef_server_url'] - The URL for the Chef server.
  # node['chef_client']['config']['validation_client_name'] - The name of the chef-validator key that is used by the chef-client to access the Chef server during the initial chef-client run.
  # node['chef_client']['config']['verbose_logging'] - Set the log level. Options: true, nil, and false. When this is set to false, notifications about individual resources being processed are suppressed (and are output at the :info logging level). Setting this to false can be useful when a chef-client is run as a daemon. Default value: nil.
  # node['chef_client']['config']['rubygems_url'] - The location to source rubygems. It can be set to a string or array of strings for URIs to set as rubygems sources. This allows individuals to setup an internal mirror of rubygems for "airgapped" environments. Default value: https://www.rubygems.org.
  #
  # See USAGE for how to set handlers with the config attribute.
