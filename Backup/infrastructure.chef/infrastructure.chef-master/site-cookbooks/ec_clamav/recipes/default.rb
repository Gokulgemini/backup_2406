include_recipe "ec_clamav::clamav-daemon"

include_recipe "ec_clamav::freshclam"

include_recipe "ec_monit_config::clamav"
