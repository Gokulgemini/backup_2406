freshclam = node['clamav']['freshclam']
clam = node['clamav']['clam']
#clam_version = node['clamav']['version']
#clam_package_name =  node['clamav']['package_name']

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
package "install clamav" do
  action :install
  package_name "clamav-freshclam"
  #version "#{clam_version}"
  notifies :restart, 'service[clamav-freshclam]', :delayed
end

# directory clam['pid_dir'] do
#   owner 'clamav'
#   group 'clamav'
# end

template freshclam['config'] do
  mode 0644
  source 'freshclam.conf.erb'
  action :create
  variables(
    :database_dir => clam['database_dir'],
    :log_file => freshclam['log_file'],
    :clamav_config => clam['config'],
    :pid_file => freshclam['pid_file']
  )
  notifies :restart, 'service[clamav-freshclam]', :delayed
end

service 'clamav-freshclam' do
  action :nothing
end
