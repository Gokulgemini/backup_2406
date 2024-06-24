name 'dpxn_service_layer_scan_ingestion'
description 'Setups up the dpxn_service_layer_scan_ingestion role'
run_list 'role[pxh_common]',
         'recipe[dpxn_service_layer_scan_ingestion]'

override_attributes "server_role": "dpxn_service_layer_scan_ingestion"
