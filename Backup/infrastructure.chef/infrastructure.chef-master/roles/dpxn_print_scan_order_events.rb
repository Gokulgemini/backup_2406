name 'dpxn_print_scan_order_events'
description 'Setups up the dpxn_print_scan_order_events role'
run_list 'role[pxh_common]',
         'role[dpxn_dotnet_base]',
         'recipe[dpxn_print_scan_order_events]'

override_attributes "server_role": "dpxn_print_scan_order_events"
