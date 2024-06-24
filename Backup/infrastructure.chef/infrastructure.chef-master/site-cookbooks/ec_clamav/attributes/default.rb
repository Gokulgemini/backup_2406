# Default Attributes for Clamav
default['clamav']['freshclam']['config'] = '/etc/clamav/freshclam.conf'
default['clamav']['freshclam']['log_file'] = '/var/log/clamav/freshclam.log'
default['clamav']['freshclam']['pid_file'] = '/var/run/clamav/freshclam.pid'

default['clamav']['clam']['config'] = '/etc/clamav/clamd.conf'
default['clamav']['clam']['database_dir'] = '/var/lib/clamav'
default['clamav']['clam']['log_file'] = '/var/log/clamav/clamd.log'
default['clamav']['clam']['pid_file'] = '/var/run/clamav/daemon-clamd.pid'
default['clamav']['clam']['pid_dir'] = '/var/run/clamav'

default['clamav']['version'] = '0.101.4+dfsg-0ubuntu0.18.04.1'
default['clamav']['package_name'] = 'clamav-daemon'
