name 'dpxn_scan_ingestion'
description 'Setups up the dpxn_scan_ingestion role'
run_list 'role[pxh_common]',
         'recipe[pen_scan_ingestion]'

override_attributes "server_role": "dpxn_scan_ingestion"
