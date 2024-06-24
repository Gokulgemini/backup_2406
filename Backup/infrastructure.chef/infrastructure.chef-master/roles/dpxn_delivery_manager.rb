name 'dpxn_delivery_manager'
description 'Setups up the dpxn_delivery_manager role'
run_list 'role[pxh_common]',
         'recipe[dpxn_delivery_manager]',
         'role[sftp_keys]'

override_attributes 'rapid7' => {
  'infrastructure_bucket' => 'dpxnpci-infra-dev'
}, 'server_role' => 'dpxn_delivery_manager'
