# ec_deluxe_sudoers CHANGELOG

This cookbook pulls the deluxe security managed sudoers file and sets up
a cron entry to do this periodically.  Also sends a web request to securities onBoarding
web service that will manage PAM and other things on the machine and update our CI with the inventoried
machine

# 0.1.2
Removing onboarding call as this is no longer necessary
# 0.1.0

Initial release.

- 2020-05-11 sync_sudo.sh moved to a template
