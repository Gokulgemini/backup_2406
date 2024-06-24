name 'dpxn_check_print'
description 'Setups up the dpxn_check_print role'
run_list 'role[pxh_common]',
         'recipe[dpxn_check_print]'

override_attributes "server_role": "dpxn_check_print"
