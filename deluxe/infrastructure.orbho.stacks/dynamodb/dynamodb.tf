module "dynamodb_table" {
  source  =  "git::ssh://git@bitbucket.org/deluxe-development/aws-dynamodb-table.git//?ref=1.0.1"
  
  name     = "results-db"
  hash_key = "PrimaryKey"
  
  attributes = var.attributes
  environment = var.environment

  region      = var.region
  point_in_time_recovery_enabled = true
  range_key = "SortKey"
  table_class = "STANDARD"

  ttl_enabled = true
  ttl_attribute_name = "Timestamp"
  server_side_encryption_enabled = true
  tags = var.tags
  replica_regions = var.replica_regions
}


