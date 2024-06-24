name 'dpxn_resque'
description 'Setups up the dpxn_resque role'
run_list 'role[pxh_common]',
         'recipe[dpxn_resque]',
         'role[sftp_keys]'

override_attributes "server_role": "dpxn_resque"
