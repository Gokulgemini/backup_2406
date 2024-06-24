name 'dpxn_cmp_mock'
description 'Setups up the dpxn_cmp_mock role'
run_list 'role[pxh_common]',
         'recipe[dpxn_cmp_mock]'

override_attributes "server_role": "dpxn_cmp_mock"
