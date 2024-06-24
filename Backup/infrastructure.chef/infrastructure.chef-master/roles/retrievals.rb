name "retrievals"

run_list "role[app_base_web]"

override_attributes "server_role": "retrievals"
