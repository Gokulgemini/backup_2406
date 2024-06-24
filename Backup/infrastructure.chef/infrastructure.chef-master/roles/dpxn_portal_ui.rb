name 'dpxn_portal_ui'
description 'Setups up the dpxn_portal_ui role'
run_list 'role[pxh_common]',
         'recipe[dpxn_portal_ui]'

override_attributes "server_role": "dpxn_portal_ui"
