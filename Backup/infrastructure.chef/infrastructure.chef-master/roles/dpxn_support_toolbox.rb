name 'dpxn_support_toolbox'
description 'Setups up the dpxn_support_toolbox role'
run_list 'role[pxh_common]',
         'role[dpxn_dotnet_base]',
         'recipe[dpxn_support_toolbox]'

override_attributes "server_role": "dpxn_support_toolbox"
