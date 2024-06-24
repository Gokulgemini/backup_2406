#
# Cookbook:: ec_mysql
# Recipe:: volume
#
# Copyright:: 2019, The Authors, All Rights Reserved.
# create a volume for tmpfs
if node[:ec_mysql][:ec2] || node[:cloud][:provider] == 'ec2' # the latter check uses Ohai's cloud detection

  # This is because nitro instances use some /dev/nvme device instead of /dev/xvd[a,b,c,d] which is specified in
  # terraform ebs volumes.  without this, linux won't see the /dev/xvd? devices for filesystem creation
  cookbook_file '/usr/sbin/ebsnvme-id' do
    mode '0755'
  end

  cookbook_file '/usr/sbin/nvme-to-block-mapping' do
    mode '0755'
  end

  execute 'nvme-to-block-mapping' do
    command '/usr/sbin/nvme-to-block-mapping'
  end

  db_tmpfs_device_id = "#{node['ec_mysql']['db_tmpfs_device']}"
  db_data_device_id = "#{node['ec_mysql']['db_data_device']}"
  db_backup_device_id = "#{node['ec_mysql']['db_backup_device']}"

#  # create a data filesystem
  execute 'db_data_mkfs' do
    command "mkfs -t ext4 #{node['ec_mysql']['db_data_device']}"
    # only if it's not mounted already
    not_if "grep -qs #{node['ec_mysql']['db_data_mountpoint']} /proc/mounts"
  end

  # now we can enable and mount it and we're done!
  directory "#{node['ec_mysql']['db_data_mountpoint']}" do
    owner 'root'
    group 'root'
    mode '0755'
    action :create
  end

  mount "#{node['ec_mysql']['db_data_mountpoint']}" do
    device node['ec_mysql']['db_data_device']
    fstype 'ext4'
    action [:enable, :mount]
  end

  # create a tmpfs filesystem
  execute 'db_tmpfs_mkfs' do
    command "mkfs -t ext4 #{node['ec_mysql']['db_tmpfs_device']}"
    # only if it's not mounted already
    not_if "grep -qs #{node['ec_mysql']['db_tmpfs_mountpoint']} /proc/mounts"
  end

  directory "#{node['ec_mysql']['db_tmpfs_mountpoint']}" do
    owner 'root'
    group 'root'
    mode '0755'
    recursive true
    action :create
  end

  mount "#{node['ec_mysql']['db_tmpfs_mountpoint']}" do
    device node['ec_mysql']['db_tmpfs_device']
    fstype 'ext4'
    #options 'noatime,nobootwait'
    action [:enable, :mount]
  end

  # create a backup filesystem
  execute 'db_backup_mkfs' do
    command "mkfs -t ext4 #{node['ec_mysql']['db_backup_device']}"
    # only if it's not mounted already
    not_if "grep -qs #{node['ec_mysql']['db_backup_mountpoint']} /proc/mounts"
  end

 directory "#{node['ec_mysql']['db_backup_mountpoint']}" do
    owner 'root'
    group 'root'
    mode '0755'
    action :create
  end

  mount "#{node['ec_mysql']['db_backup_mountpoint']}" do
    device node['ec_mysql']['db_backup_device']
    fstype 'ext4'
    #options 'noatime,nobootwait'
    action [:enable, :mount]
  end

end
