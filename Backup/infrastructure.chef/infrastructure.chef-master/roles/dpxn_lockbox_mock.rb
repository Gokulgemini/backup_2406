name 'dpxn_lockbox_mock'
description 'Setups up the dpxn_lockbox_mock role'
run_list 'role[pxh_common]',
         'recipe[dpxn_lockbox_mock]'

override_attributes "server_role": "dpxn_lockbox_mock"