apt_repository 'universe' do
  uri        "http://archive.ubuntu.com/ubuntu"
  components ['universe']
  action     :add
  only_if { platform?('ubuntu') }
end

remote_file "/tmp/libssl1.1_1.1.1f-1ubuntu2.22_amd64.deb" do
  source "http://archive.ubuntu.com/ubuntu/pool/main/o/openssl/libssl1.1_1.1.1f-1ubuntu2.22_amd64.deb"
  mode 0777
  action :create
  checksum "df9d07d552aab0c7e5b9fbcc568913acd20d50fb8b1e34876fa348b7a0c82d48"
  not_if "dpkg -s libssl1.1 | grep 'Version: 1.1.1f-1ubuntu2.22'"
  notifies :run, 'execute[apt install libssl1.1]', :immediately
end

execute 'apt install libssl1.1' do
  action :run
  not_if "dpkg -s libssl1.1 | grep 'Version: 1.1.1f-1ubuntu2.22'"
  command 'apt install /tmp/libssl1.1_1.1.1f-1ubuntu2.22_amd64.deb -y'
end

remote_file "/tmp/wkhtmltox_0.12.1.4-2.bionic_amd64.deb" do
  source "https://github.com/wkhtmltopdf/packaging/releases/download/0.12.1.4-2/wkhtmltox_0.12.1.4-2.bionic_amd64.deb"
  mode 0777
  action :create
  checksum "7186e124bf04d68dab48e4719f79d7767d4e2615d1688297e6acd5f508b59c9e"
  not_if "dpkg -s wkhtmltox | grep 'Version: 1:0.12.1.4-2.bionic'"
  notifies :run, 'execute[apt install wkhtmltox]', :immediately
end

execute 'apt install wkhtmltox' do
  action :run
  not_if "dpkg -s wkhtmltox | grep 'Version: 1:0.12.1.4-2.bionic'"
  command 'apt install /tmp/wkhtmltox_0.12.1.4-2.bionic_amd64.deb -y'
end

node['ec_security_updates']['packages'].each do |package_data|
  package package_data[:name] do
    %w{version source options action}.each do |attr|
      send(attr, package_data[attr]) if package_data[attr]
    end
  end
end
