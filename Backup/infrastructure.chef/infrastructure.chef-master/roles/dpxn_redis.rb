name 'dpxn_redis'
description 'Setups up the dpxn_redis role'
run_list 'recipe[dpxn_redis]'

override_attributes "server_role": "dpxn_redis"
