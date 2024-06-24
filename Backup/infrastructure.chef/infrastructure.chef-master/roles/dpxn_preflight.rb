name 'dpxn_preflight'
description 'Setups up the dpxn_preflight role'
run_list 'role[pxh_common]',
         'recipe[dpxn_preflight]'

override_attributes "server_role": "dpxn_preflight"
