name 'dpxn_print_scan_ingestion'
description 'Setups up the dpxn_print_scan_ingestion role'
run_list 'role[pxh_common]',
         'role[dpxn_dotnet_base]',
         'recipe[dpxn_print_scan_ingestion]'

override_attributes "server_role": "dpxn_print_scan_ingestion"
