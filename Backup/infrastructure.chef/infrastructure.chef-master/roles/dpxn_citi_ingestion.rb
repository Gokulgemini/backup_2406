name 'dpxn_citi_ingestion'
description 'Setups up the dpxn_citi_ingestion role'
run_list 'role[pxh_common]',
         'recipe[dpxn_citi_ingestion]'

override_attributes "server_role": "dpxn_citi_ingestion"
