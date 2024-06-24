name 'dpx_redis'
description 'Setups up the dpx_redis role'
run_list 'recipe[dpx_redis]'

override_attributes "server_role": "redis"
