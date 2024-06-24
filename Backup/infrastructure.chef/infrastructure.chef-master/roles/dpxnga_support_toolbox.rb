name 'dpxnga_support_toolbox'
description 'Setups up the dpxnga_support_toolbox role'
run_list 'role[pxh_common]',
         'role[dpxn_dotnet_base]',
         'recipe[dpxn_support_toolbox]'

override_attributes "server_role": "dpxnga_support_toolbox"
