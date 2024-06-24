
if node['rails_env'].nil? && node['app_env'].nil?
  environment = node['ec_prompt']['rails_env']
elsif node['app_env'].nil?
  environment = node['rails_env']
else
  environment = node['app_env']
end

role = node['server_role'].nil? ? node['ec_prompt']['server_role'] : node['server_role']

template "/etc/profile.d/custom_env_vars.sh" do
  source "custom_env_vars.sh.erb"
  variables(
    :rails_env => environment,
    :role => role,
    :chef_env => node['chef_environment']
  )
end

template "/etc/profile.d/custom_prompt.sh" do
  source "custom_prompt.sh.erb"
end

# Replace the skeleton bashrc for all new user accounts
template "/etc/skel/.bashrc" do
  source "skel_bashrc.erb"
end

# Fix the ubuntu user account. Because it is created before we overwrite the skeleton
# I know this is ugly
template "/home/ubuntu/.bashrc" do
  source "skel_bashrc.erb"
end
