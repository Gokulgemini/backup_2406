name 'dpxn_print_scan_notifications'
description 'Setups up the dpxn_print_scan_notifications role'
run_list 'role[pxh_common]',
         'role[dpxn_dotnet_base]',
         'recipe[dpxn_print_scan_notifications]'

override_attributes "server_role": "dpxn_print_scan_notifications"
