name 'dpxn_check_order'
description 'Setups up the dpxn_check_order role'
run_list 'role[pxh_common]',
         'role[dpxn_dotnet_base]',
         'recipe[dpxn_check_order]'

override_attributes "server_role": "dpxn_check_order"
