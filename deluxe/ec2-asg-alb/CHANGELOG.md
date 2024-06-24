# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## 2.0.0 - 2024-03-04
- Added support for EC2 launch templates since AWS deprecated the feature of AWS EC2 launch configuration.
- Added additional output variables which required for terragrunt module.
- Refer - https://docs.aws.amazon.com/autoscaling/ec2/userguide/launch-configurations.html
- Domain join user data script has been updated.

## 1.14.4 - 2023-09-06
- Adding output variable for ec2 instance private ip.
- Adding output variable for ALB target group arn.

## 1.14.3 - 2023-06-20
- Adding output variable for private dns name.

## 1.14.2 - 2023-06-07
- Wrapping user-data local initialization in try, as it is not required when only ALB is being created.
- Adding output variable for instance_id.

## 1.14.1 - 2023-06-05
- Guard AMI search execution.  Only run when we have a filter string supplied.

## 1.14.0 - 2023-06-02
- Add lifecycle ignore changes for tags on ec2 resource .

## 1.13.0 - 2023-05-24
- Adding support to accept configuration of ALB access-logs. Refer 'alb_access_logs' in the example mentioned in readme.
- Adding LB name in output.
- Restricting the major version of aws provider to be used as 4.x because there are breaking changes in newly released 5.x version.

## 1.12.0 - 2023-05-04
- Fixed an issue while creating instance without ASG but with domain join.
- Reading user-data via time_static asg only. To avoid mismatch between time_static & ec2 in case when user_data_base64 is provided by consumers

## 1.11.1 - 2023-04-19
- Fixed the asg min/max size variables mapping. They were interchanged previously.
- Terraform version constraint removed. Live modules can now pin the exact versoin they want to use.

## 1.11.0 - 2022-10-31
- Added WinRM HTTPS listener to userdata script
- Added restart after ServerBuild to prevent needing manual restart on script failure / hang

## 1.10.0 - 2022-10-11
### Added
- Added fixes for existing variables.

## 1.9.0 - 2022-10-06
### Added
- Added ability to change Windows version in userdata script.

## 1.8.0 - 2022-05-12
### Added
- Added EC2 module and tweaked existing terraform code.

## 1.7.0 - 2022-02-15
### Added
- Added DBA accounts to local admin list

## 1.6.0 - 2022-02-09
### Added
- added http_tcp_listener_rules and https_listener_rules for routing traffic with ALB
- Updated source module version for ALB from "5.0" to "6.7.0"

## 1.5.0 - 2022-01-28
### Added
- aws-ec2-asg-alb module added that can be able to create Ec2 instance using Auto scaling group and ALB
- added user data script for join the instance in deluxedomain
