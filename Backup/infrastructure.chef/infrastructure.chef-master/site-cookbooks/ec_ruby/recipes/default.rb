# Ruby changed up their ftp repo, need to use major/minor version in path
ruby_base_version = node['ec_ruby']['version'].reverse.split('.', 2).collect(&:reverse).reverse.first

package 'ruby' do
  action :purge
  not_if "ruby -v | grep #{node['ec_ruby']['version']}"
end

execute "fix broken packages Ubuntu 22.04" do
  command "apt install --fix-broken"
end  

build_dependencies = [ 'autoconf', 'bison', 'build-essential', 'libyaml-dev', 'libreadline-dev', 'zlib1g-dev', 'libncurses5-dev', 'libffi-dev', 'libgdbm-dev', 'libssl-dev', 'libmysqlclient-dev', 'libmagickwand-dev']

build_dependencies.each do |package|
  package package
end

remote_file "#{Chef::Config['file_cache_path']}/ruby-#{node['ec_ruby']['version']}.tar.gz" do
  source "https://ftp.ruby-lang.org/pub/ruby/#{ruby_base_version}/ruby-#{node['ec_ruby']['version']}.tar.gz"
  mode '0755'
  action :create
  not_if "ruby -v | grep #{node['ec_ruby']['version']}"
  #notifies :delete, this, :immediate
  notifies :run, 'execute[extract_ruby]', :immediate
end

execute "extract_ruby" do
  action :nothing
  cwd "#{Chef::Config['file_cache_path']}"
  command "tar xvfz #{Chef::Config['file_cache_path']}/ruby-#{node['ec_ruby']['version']}.tar.gz"
  creates "#{Chef::Config['file_cache_path']}/ruby-#{node['ec_ruby']['version']}"
  notifies :run, 'execute[build_ruby]', :immediate
end

execute "build_ruby" do
  action :nothing
  cwd "#{Chef::Config['file_cache_path']}/ruby-#{node['ec_ruby']['version']}"
  command "./configure && make && make install"
  #not_if "ruby -v | grep #{node['ec_ruby']['version']}"
end

file 'cleanup_download' do
  path "#{Chef::Config['file_cache_path']}/ruby-#{node['ec_ruby']['version']}.tar.gz"
  action :delete
end

directory 'cleanup_dir' do
  recursive true
  path "#{Chef::Config['file_cache_path']}/ruby-#{node['ec_ruby']['version']}"
  action :delete
end

