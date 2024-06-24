name 'pxh_api'
description 'Setups up the pxh_api role'
run_list 'role[pxh_common]',
         'recipe[pxh_api]'

override_attributes "server_role": "dpxn_api"
