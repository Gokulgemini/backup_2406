# version to install
default['ec_mysql']['version'] = '5.7'

# mysql tempfs
default[:ec_mysql]['db_tmpfs_mountpoint'] = '/tempfs'
default[:ec_mysql]['db_tmpfs_size'] = 32
default[:ec_mysql]['db_tmpfs_device'] = '/dev/xvdd'

# mysql data
default[:ec_mysql]['db_data_mountpoint'] = '/data'
default[:ec_mysql]['db_data_size'] = 600
default[:ec_mysql]['db_data_device'] = '/dev/xvdb'

# mysql backup
default[:ec_mysql]['db_backup_mountpoint'] = '/backup'
default[:ec_mysql]['db_backup_size'] = 1229
default[:ec_mysql]['db_backup_device'] = '/dev/xvdc'

# client
default['ec_mysql']['client']['port'] = '3306'

# mysqladmin
default['ec_mysql']['mysqladmin']['character-sets-dir'] = '/usr/share/mysql/charsets'
default['ec_mysql']['mysqladmin']['default-character-set'] = 'utf8'

# mysqlcheck
default['ec_mysql']['mysqlcheck']['character-sets-dir'] = '/usr/share/mysql/charsets'
default['ec_mysql']['mysqlcheck']['default-character-set'] = 'utf8'

# mysqlimport
default['ec_mysql']['mysqlimport']['character-sets-dir'] = '/usr/share/mysql/charsets'
default['ec_mysql']['mysqlimport']['default-character-set'] = 'utf8'

# mysqlshow
default['ec_mysql']['mysqlshow']['character-sets-dir'] = '/usr/share/mysql/charsets'
default['ec_mysql']['mysqlshow']['default-character-set'] = 'utf8'

# myisampack
default['ec_mysql']['myisampack']['character-sets-dir'] = '/usr/share/mysql/charsets'


# mysqld_safe
#default['ec_mysql'][''][''] = ''

# mysqld configuration
default['ec_mysql']['mysqld']['port'] = '3306'
default['ec_mysql']['mysqld']['bind_address'] = '0.0.0.0'
default['ec_mysql']['mysqld']['pid_file'] = '/var/run/mysqld/mysqld.pid'
default['ec_mysql']['mysqld']['socket'] = '/var/run/mysqld/mysqld.sock'
default['ec_mysql']['mysqld']['binlog_format'] = 'MIXED'
default['ec_mysql']['mysqld']['max_allowed_packet'] = '64M'
default['ec_mysql']['mysqld']['character_set_server'] = 'utf8'
default['ec_mysql']['mysqld']['key_buffer_size'] = '32M'
default['ec_mysql']['mysqld']['thread_cache_size'] = '512'
default['ec_mysql']['mysqld']['explicit_defaults_for_timestamp'] = true
default['ec_mysql']['mysqld']['performance_schema'] = '0'
default['ec_mysql']['mysqld']['sort_buffer_size'] = '2M'
default['ec_mysql']['mysqld']['net_buffer_length'] = '64K'
default['ec_mysql']['mysqld']['read_buffer_size'] = '1M'
default['ec_mysql']['mysqld']['read_rnd_buffer_size'] = '1M'
default['ec_mysql']['mysqld']['myisam_sort_buffer_size'] = '2M'
default['ec_mysql']['mysqld']['log-bin-trust-function-creators'] =  true
default['ec_mysql']['mysqld']['tmpdir'] =  '/tempfs'
default['ec_mysql']['mysqld']['query_cache_size'] =  '16M'
default['ec_mysql']['mysqld']['query_cache_type'] =  '1'

# mysqldump configuration
default['ec_mysql']['mysqldump']['quick'] =  true
default['ec_mysql']['mysqldump']['max_allowed_packet'] = '128M'
default['ec_mysql']['mysqldump']['hex_blob'] =  true
default['ec_mysql']['mysqldump']['character-sets-dir'] = '/usr/share/mysql/charsets'
default['ec_mysql']['mysqldump']['default-character-set'] = 'utf8'

# mysql
default['ec_mysql']['mysql']['safe_updates'] =  false
default['ec_mysql']['mysql']['character-sets-dir'] = '/usr/share/mysql/charsets'
default['ec_mysql']['mysql']['default-character-set'] = 'utf8'

# isamchk
default['ec_mysql']['isamchk']['key_buffer'] = '20M'
default['ec_mysql']['isamchk']['sort_buffer_size'] = '20M'
default['ec_mysql']['isamchk']['read_buffer'] = '2M'
default['ec_mysql']['isamchk']['write_buffer'] = '2M'

# myisamchk
default['ec_mysql']['myisamchk']['character-sets-dir'] = '/usr/share/mysql/charsets'
default['ec_mysql']['myisamchk']['key_buffer'] = '20M'
default['ec_mysql']['myisamchk']['sort_buffer_size'] = '20M'
default['ec_mysql']['myisamchk']['read_buffer'] = '2M'
default['ec_mysql']['myisamchk']['write_buffer'] = '2M'
default['ec_mysql']['myisamchk']['ft_min_word_len'] = '3'

# mysqlhotcopy
default['ec_mysql']['mysqlhotcopy']['interactive-timeout'] = true
