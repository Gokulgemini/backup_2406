#freshclam = node['clamav']['freshclam']
clam = node['clamav']['clam']
#clam_version = node['clamav']['version']
clam_package_name =  node['clamav']['package_name']

# # Lets first check if the package is installed with the wrong version
# #If so uninstall
# package clam_package_name do
#   action :remove
#   not_if "eix #{clam_package_name} -v | grep 'Version:' | grep '#{clam_version}' "
# end

#refactored by commenting out
# enable_package clam_package_name do
#   version clam_version
#   unmask true
#   override hardmask true
# end

#refactored by commenting out
# update_file "local portage package.keywords" do
#   path "/etc/portage/package.keywords/local"
#   body "=#{clam_package_name}-#{clam_version}"
#   not_if "grep '#{clam_package_name}' /etc/portage/package.keywords/local"
# end

#refactored by commenting out
# execute "installing app-antivirus/clamav" do
#   command "EXTRA_ECONF='--disable-zlib-vcheck' emerge  =#{clam_package_name}-#{clam_version}"
#   not_if "eix #{clam_package_name} -v | grep 'Version:' | grep '#{clam_version}' "
# end

#added this:
package "install clamav daemon" do
  action :install
  package_name "#{clam_package_name}"
  #version "#{clam_version}"
  notifies :create, 'directory[/run/clamav]', :immediately
  #why won't the directory get created???
  notifies :restart, 'service[clamav-daemon]', :delayed
end

file "/etc/systemd/system/clamav-daemon.service.d/extend.conf" do
  action :delete
  notifies :run, 'execute[daemon-reload for clamav-daemon]', :immediately
end

directory "/run/clamav" do
  path "/run/clamav"
  owner 'clamav'
  group 'clamav'
  mode '0755'
  action :create
end

# directory clam['pid_dir'] do
#   owner 'clamav'
#   group 'clamav'
# end

template clam['config'] do
  mode 0644
  source 'clamd.conf.erb'
  action :create
  variables(
    :database_dir => clam['database_dir'],
    :log_file => clam['log_file'],
    :pid_file => clam['pid_file']
  )
end

template "/etc/apparmor.d/local/usr.sbin.clamd" do
  mode 0644
  source 'local.usr.sbin.clamd.erb'
  action :create
  notifies :run, 'execute[reload clamd apparmor profile]', :immediately
end

execute 'reload clamd apparmor profile' do
  action :nothing
  command 'apparmor_parser -r /etc/apparmor.d/usr.sbin.clamd'
  notifies :restart, 'service[clamav-daemon]', :delayed
end

template "/lib/systemd/system/clamav-daemon.service" do
  mode 0644
  source 'clamav-daemon.service.erb'
  action :create
  notifies :run, 'execute[daemon-reload for clamav-daemon]', :immediately
end

# apparmor_profile = "/etc/apparmor.d/usr.sbin.clamd"
# commented_include = /\s#include\s<local\/usr\.sbin\.clamd>/
#
# ruby_block "include local config for clamd apparmor profile" do
#   block do
#     fe = Chef::Util::FileEdit.new(apparmor_profile)
#     fe.search_file_replace_line(commented_include, "include <local/usr.sbin.clamd>")
#     fe.write_file
#   end
#   only_if { ::File.readlines(apparmor_profile).grep(commented_include).any? }
# end if platform_family?('debian')

execute "daemon-reload for clamav-daemon" do
  command 'systemctl daemon-reload'
  action :nothing
  notifies :restart, 'service[clamav-daemon]', :delayed
end

service 'clamav-daemon' do
  action :nothing
end
