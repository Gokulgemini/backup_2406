name 'dpxn_resque_ui'
description 'Setups up the dpxn_resque_ui role'
run_list 'recipe[dpxn_resque_ui]'

override_attributes "server_role": "dpxn_resque_ui"
