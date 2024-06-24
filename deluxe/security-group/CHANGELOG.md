# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]


## 1.3.0 - 2023-11-16
### Added
- Removed version from provider section to use version from upstream module. 

## 1.2.0 - 2023-10-18
### Added
- bug fix to support ingress_with_source_security_group_id

## 1.1.0 - 2023-09-05
### Added
- Use try blocks in the event that ingress with cidr blocks doesn't exist.
- Added support for ingress_with_source_security_group_id .

## 1.0.0 - 2022-02-04
### Added
- aws-security-groups module added that can be create VPC security groups which can be used for any ec2 instances.
