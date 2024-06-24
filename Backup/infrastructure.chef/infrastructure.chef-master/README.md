# eChecks Cookbooks

This repo contains the cookbooks required to set up eChecks.com on Deluxe Golden landing zone

### Setup WorkStation
In order to setup a workstation so you can interact with the chef server and upload cookbooks you have to do the following.

1. Download the [Chef WorkStation](https://downloads.chef.io/chef-workstation/#mac_os_x). At the time of this document it is version 0.12.20
2. Once you have chef workstation installed go into the root of this repo once you have cloned it locally.
3. Install coreutils first. `brew install coreutils'`. Run `bundle install`
4. Create the following directory in your home director.  `.chef`
5. At this point you will have to add your a couple of required files. You will need to get you pem file generated when your user was created and the validator pem for the org. The directory structure will look like the following.
```bash
/Users/t450963/.chef/
├── config.rb
├── david_rodriguez.pem
├── echecks-dev-validator.pem
```
6. `config.rb` will need to look like the following.
```bash
 current_dir = "<location of .chef directory>"
log_level                :info
log_location             STDOUT
node_name                "<username>"
client_key               "#{current_dir}/<user pem>"
chef_server_url          "https://echecks-chef-server.deluxe.com/organizations/<org_we_are_dealing_with>"
cookbook_path            ["<location_of_chef_repo>/site-cookbooks"]
```
7. Download the ssl certs from chef server. We do this so knife doesn't have issues communicating with the chef server. `knife ssl fetch` This will create a directory with the required certificates.  Your `.chef` directory will look like the following .
```bash
t450963: ~/Documents/git/infrastructure.chef (update_with_workstation_setup) $ tree ~/.chef/
/Users/t450963/.chef/
├── config.rb
├── david_rodriguez.pem
├── echecks-dev-validator.pem
└── trusted_certs
    ├── Deluxe_Enterprise_CA.crt
    ├── Deluxe_Root_CA.crt
    └── echecks-chef-server_deluxe_com.crt
```

### Seeding cookbooks
8. Ensure you have bundler installed
```bash
$ gem install bundler:1.17.3
```
9. Verify knife is working correctly
```bash
~/infrastructure.chef
$ chef exec knife node list
```
Result might be empty or chef license acceptance. This might indicate that CHEF_ORG might not be set correctly.
Set the env variable to the correct organization (from Chef) for seeding cookbooks. Set only one.
```bash
$ export CHEF_ORG=dpxn 
$ export CHEF_ORG=dpxn-dev
```
10. Ensure the repo is checked out to latest stable branch. We will be using "berks" tool for seeding. It will read the BERKS file for dependencies of cookbooks.
```bash
~/infrastructure.chef
$ chef exec berks vendor cookbooks
Chef::Exceptions::CookbookNotFoundInRepo The directory ~/site-cookbooks/ec_netowl does not contain a cookbook
```
Sample Berksfile:
```
~/infrastructure.chef
$ cat Berksfile
source 'https://supermarket.chef.io'
cookbook 'ec_app_config', path: './site-cookbooks/ec_app_config'
```

11. As the first time run, you might not have other cookbooks cloned on the machine. Let's first run the clone_cookbooks.sh found in bin
```bash
~/infrastructure.chef
$ ./bin/clone_cookbooks.sh
```
12. Once the cloning of all cookboks complete, you can now proceed with seeding (uploading) cookbooks to the new Chef server. Ensure you know which cookbooks require seeding. We will be using berks cmd for seeding. Might be required to run "berks install" first to ensure dependencies are resolved before update and upload.
** Please note these steps are required for each cookbook you plan to seed **
```bash
~/infrastructure.chef/site-cookbooks/role_pxh_common
$ chef exec berks install
Resolving cookbook dependencies...
Fetching 'dpx_filesystem' from ssh://git@bitbucket.deluxe.com:7999/echecks/infrastructure.chef.dpx_filesystem.git (at master)
```
13. 
```bash
~/infrastructure.chef/site-cookbooks/role_pxh_common
$ chef exec berks upload
Uploaded lvm (5.0.6) to: 'https://chef-west.gs-echecks.com/organizations/dpxn'
Uploaded filesystem (3.0.3) to: 'https://chef-west.gs-echecks.com/organizations/dpxn'
```

14. Finally you would also want to upload the roles for these cookbooks
```bash
~/infrastructure.chef
$ chef exec knife role from file roles/pxh_common.rb
Updated Role pxh_common
```
