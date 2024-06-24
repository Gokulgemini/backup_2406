name 'netowl'
description 'configures netowl utility role'
run_list 'recipe[netowl]'

override_attributes "server_role": "netowl"
