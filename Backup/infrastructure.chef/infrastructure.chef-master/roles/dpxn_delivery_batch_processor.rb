name 'dpxn_delivery_batch_processor'
description 'Setups up the dpxn_delivery_batch_processor role'
run_list 'role[pxh_common]',
         'recipe[pen_delivery_batch_processor]',
         'role[dpxn_cron]'

override_attributes "server_role": "dpxn_delivery_batch_processor"
