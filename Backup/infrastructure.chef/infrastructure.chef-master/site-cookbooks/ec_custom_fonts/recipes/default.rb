# #
# # Cookbook Name:: fonts
# # Recipe:: default
# #
#

# We are making an assumption that our image magick home directory will not
# change.
imagemagick_home = '/etc/ImageMagick-6/'

apt_package 'imagemagick-6.q16' do
  action :upgrade
  options '-f'
end

cookbook_file "#{imagemagick_home}/policy.xml" do
  source 'policy.xml'
  mode '0644'
  action :create
end

windows_type_path = '/etc/ImageMagick-6/type-windows.xml'

type_path = '/etc/ImageMagick-6/type.xml'

directory '/usr/share/fonts/corefonts/' do
  mode '0755'
  action :create
end

directory '/usr/share/fonts/default/' do
  mode '0755'
  action :create
end

bash 'Install Mrs. Saint Delafield (corefont)' do
  cwd '/usr/share/fonts/corefonts/'
  code <<-EOH
    wget #{node['fonts']['mrs_saint_delafield_s3_url']}
  EOH
  not_if { File.exists?("/usr/share/fonts/corefonts/#{node['fonts']['mrs_saint_delafield_file_name']}") }
end

bash 'install Micr Font ttf - corefont' do
  cwd '/usr/share/fonts/corefonts/'
  code <<-EOH
    wget #{node['fonts']['micr_ttf_s3_url']}
  EOH
  not_if { File.exists?("/usr/share/fonts/corefonts/#{node['fonts']['micr_ttf_file_name']}") }
end

bash 'install Micr Font otf - corefont' do
  cwd '/usr/share/fonts/corefonts/'
  code <<-EOH
    wget #{node['fonts']['micr_otf_s3_url']}
  EOH
  not_if { File.exists?("/usr/share/fonts/corefonts/#{node['fonts']['micr_otf_file_name']}") }
end

bash 'install Micr Font otf - default font' do
  cwd '/usr/share/fonts/default/'
  code <<-EOH
    wget #{node['fonts']['micr_otf_s3_url']}
  EOH
  not_if { File.exists?("/usr/share/fonts/default/#{node['fonts']['micr_otf_file_name']}") }
end

ruby_block 'append xml to type-windows.xml - micr font' do
 block do
  #execute 'append xml to type-windows.xml - micr font' do
    windows_font_xml_file = File.open(windows_type_path, 'rb')
    correct_xml_file = File.open('/tmp/type-windows.xml', 'w')
    mrs_saint_delafield_xml_statement = '<type name="MrsSaintDelafield-Regular" fullname="MrsSaintDelafield-Regular" family="MrsSaintDelafield" weight="400" style="normal" stretch="normal" glyphs="/usr/share/fonts/corefonts/MrsSaintDelafield-Regular.ttf" encoding="AppleRoman"/>'

    windows_font_xml_file.each do |line|
      # the file ends with "</typemap>" and we need to add our xml right before it
      if line.include?('</typemap>')
        correct_xml_file.write(mrs_saint_delafield_xml_statement + "\n")
      end
      correct_xml_file.write(line)
    end

    # close up our files so we dont leak memory
    windows_font_xml_file.close
    correct_xml_file.close
    FileUtils.mv('/tmp/type-windows.xml', windows_type_path)

   # command %Q{sudo mv -f '/tmp/type-windows.xml' '#{windows_type_path}'}

 # end
 end
 not_if { File.read(windows_type_path).include?('MrsSaintDelafield-Regular') }
 action :run
end

ruby_block 'register windows font file' do
 block do
  #execute 'register windows font file' do

    font_xml_file = File.open(type_path, 'rb')
    correct_xml_file = File.open('/tmp/type.xml', 'w')

    font_xml_file.each do |line|
      # the file ends with "</typemap>" and we need to add our xml right before it
      if line.include?('</typemap>')
        correct_xml_file.write('<include file="type-windows.xml" />' + "\n")
      end
      correct_xml_file.write(line)
    end

    # close up our files so we dont leak memory
    font_xml_file.close
    correct_xml_file.close
    FileUtils.mv('/tmp/type.xml', type_path)
   # command %Q{sudo mv -f '/tmp/type.xml' '#{type_path}'}
    #not_if { File.read(type_path).include?('type-windows.xml') }
  #end
 end
 action :run
 not_if { File.read(type_path).include?('type-windows.xml') }
end
