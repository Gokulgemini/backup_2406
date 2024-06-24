name 'dpxn_check_ingestion'
description 'Setups up the dpxn_check_ingestion role'
run_list 'role[pxh_common]',
         'recipe[pen_check_ingestion]'

override_attributes "server_role": "dpxn_check_ingestion"
