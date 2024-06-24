name 'dpxn_delivery_batcher'
description 'Setups up the dpxn_delivery_batcher role'
run_list 'role[pxh_common]',
         'recipe[pen_delivery_batcher]'

override_attributes "server_role": "dpxn_delivery_batcher"
