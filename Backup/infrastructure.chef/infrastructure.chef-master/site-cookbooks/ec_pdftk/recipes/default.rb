#
# Cookbook:: ec_pdftk
# Recipe:: default
#
# Copyright:: 2020, The Authors, All Rights Reserved.

# https://itectec.com/ubuntu/ubuntu-how-to-install-pdftk-in-ubuntu-18-04-and-later/
# https://gitlab.com/pdftk-java/pdftk

pdftk = node['pdftk']

package 'Install pdftk' do
  package_name 'pdftk'
  version      pdftk['version']
  action       :install
end
