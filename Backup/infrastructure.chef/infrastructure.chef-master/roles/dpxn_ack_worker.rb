name 'dpxn_ack_worker'
description 'Setups up the dpxn_ack_worker role'
run_list 'role[pxh_common]',
         'recipe[pen_ack_worker]'

override_attributes "server_role": "dpxn_ack_worker"
