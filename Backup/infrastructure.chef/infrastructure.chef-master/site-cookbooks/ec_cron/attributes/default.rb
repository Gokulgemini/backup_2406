# frozen_string_literal: true

default['ec_cron']['app_name'] = 'default'
default['ec_cron']['environment'] = 'default'
default['ec_cron']['ec_cron_log'] = '/home/deploy/ec_cron.log'
default['ec_cron']['ec_cleanup_conf'] = '/etc/rsyslog.d/ec_cleanup.conf'

# Add crons here
default['ec_cron']['echeck_crons'] = [
  { name: 'fspace_tmp_dir', time: '10 03 * * *', command: 'cd /data && find . -maxdepth 2 -name "shared" | xargs -I {} find "{}" -maxdepth 1 -name tmp | xargs -I {} find "{}" -maxdepth 1 -name "*.*" -ctime +1 -print0 | xargs -0 rm' },
  { name: 'fspace_tmp_checks_dir', time: '15 03 * * *', command: 'cd /data && find . -maxdepth 2 -name "shared" | xargs -I {} find "{}" -maxdepth 1 -name tmp | xargs -I {} find "{}" -maxdepth 1 -name checks  | xargs -I {} find "{}" -maxdepth 4 -name "*.*" -ctime +2 -print0 | xargs -0 rm' },
  { name: 'fspace_tmp_checks_old_dir', time: '30 03 * * *', command: 'cd /data && find . -maxdepth 2 -name "shared" | xargs -I shared_dir find "shared_dir" -maxdepth 1 -name tmp | xargs -I tmp_dir find "tmp_dir" -maxdepth 1 -name checks -type d | xargs -I check_dirs find "check_dirs" -maxdepth 4 -ctime +2 -print0 -type d -empty | xargs -0 -I check_dir rm -r "check_dir"' },
  { name: 'fspace_tmp_uploads_files', time: '00 04 * * *', command: 'cd /data && find . -maxdepth 2 -name "current" | xargs -I {} find -L "{}" -maxdepth 1 -name public | xargs -I {} find "{}" -maxdepth 2 -name tmp | xargs -I {} find "{}" -maxdepth 4 -name "*.*" -ctime +2 -print0  | xargs -0 rm' },
  { name: 'fspace_tmp_variable_print_files', time: '01 04 * * *', command: 'cd /data && find . -maxdepth 2 -name "shared" | xargs -I {} find "{}" -maxdepth 1 -name tmp | xargs -I {} find "{}" -maxdepth 1 -name variable_print  | xargs -I {} find "{}" -maxdepth 4 -name "*.*" -ctime +2 -print0 | xargs -0 rm' },
  { name: 'remove_tmp_files', time: '10 00 * * *', command: 'cd /data && find . -maxdepth 2 -name "releases" | xargs -I shared_dir find "shared_dir" -maxdepth 2 -name public | xargs -I tmp_dir find "tmp_dir" -maxdepth 2 -name tmp -type d | xargs -I {} find "{}" -maxdepth 4 -ctime +2 -type f -exec rm -rf {} \;' },
  { name: 'remove_tmp_dirs', time: '15 00 * * *', command: 'cd /data && find . -maxdepth 2 -name "releases" | xargs -I shared_dir find "shared_dir" -maxdepth 2 -name public | xargs -I tmp_dir find "tmp_dir" -maxdepth 2 -name tmp -type d | xargs -I check_dirs find "check_dirs" -maxdepth 4 -ctime +2 -print0 -type d -empty | xargs -0 -I check_dir rm -rf "check_dir"' },
  { name: 'remove_bootsnap_dirs', time: '0 3 * * 7', command: 'cd /data && find . -maxdepth 5 -name "bootsnap*" | xargs -I bootsnap_dir rm -rf "bootsnap_dir"' }
]

# Only apply to production env
# if node['ec_cron']['environment'] == 'production'
#   default[:custom_crons] << {:name => 'DeadmanSnitch', :time => '28,58 * * * *', :command => 'curl https://nosnch.in/4c327f4bc4'}
# end
#

default['ec_cron']['delayed_job_crons'] = [
  { name: 'vv_moniting_hourly_tasks', time: '0 * * * *', command: "/bin/bash -l -c 'cd /data/monitoring/current && RAILS_ENV=lz bundle exec rake tasks:hourly --silent'" },
  { name: 'vv_moniting_daily_tasks', time: '1 0 * * *', command: "/bin/bash -l -c 'cd /data/monitoring/current && RAILS_ENV=lz bundle exec rake tasks:daily --silent'" },
  { name: 'fspace_tmp_dir', time: '10 03 * * *', command: 'cd /data && find . -maxdepth 2 -name "shared" | xargs -I {} find "{}" -maxdepth 1 -name tmp | xargs -I {} find "{}" -maxdepth 1 -name "*.*" -ctime +1 -print0 | xargs -0 rm' },
  { name: 'fspace_tmp_checks_dir', time: '15 03 * * *', command: 'cd /data && find . -maxdepth 2 -name "shared" | xargs -I {} find "{}" -maxdepth 1 -name tmp | xargs -I {} find "{}" -maxdepth 1 -name checks  | xargs -I {} find "{}" -maxdepth 4 -name "*.*" -ctime +2 -print0 | xargs -0 rm' },
  { name: 'fspace_tmp_checks_old_dir', time: '30 03 * * *', command: 'cd /data && find . -maxdepth 2 -name "shared" | xargs -I shared_dir find "shared_dir" -maxdepth 1 -name tmp | xargs -I tmp_dir find "tmp_dir" -maxdepth 1 -name checks -type d | xargs -I check_dirs find "check_dirs" -maxdepth 4 -ctime +2 -print0 -type d -empty | xargs -0 -I check_dir rm -r "check_dir"' },
  { name: 'fspace_tmp_uploads_files', time: '00 04 * * *', command: 'cd /data && find . -maxdepth 2 -name "current" | xargs -I {} find -L "{}" -maxdepth 1 -name public | xargs -I {} find "{}" -maxdepth 2 -name tmp | xargs -I {} find "{}" -maxdepth 4 -name "*.*" -ctime +2 -print0  | xargs -0 rm' },
  { name: 'fspace_tmp_variable_print_files', time: '01 04 * * *', command: 'cd /data && find . -maxdepth 2 -name "shared" | xargs -I {} find "{}" -maxdepth 1 -name tmp | xargs -I {} find "{}" -maxdepth 1 -name variable_print  | xargs -I {} find "{}" -maxdepth 4 -name "*.*" -ctime +2 -print0 | xargs -0 rm' }
]
