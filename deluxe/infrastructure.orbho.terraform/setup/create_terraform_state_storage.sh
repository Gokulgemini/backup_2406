#!/usr/bin/env bash
bucket_name=$1
profile_name=$2
region_name=$3

if [[ ! -z $bucket_name && ! -z $profile_name && ! -z $region_name ]];
then
  aws s3 mb s3://$bucket_name --profile $profile_name --region $region_name

  aws s3api put-bucket-versioning --bucket $bucket_name --profile $profile_name  --region $region_name --versioning-configuration Status=Enabled

 # aws s3api put-bucket-acl --bucket $bucket_name --profile $profile_name --region $region_name --acl private

  aws s3api put-public-access-block \
    --bucket $bucket_name \
    --profile $profile_name \
    --region $region_name \
    --public-access-block-configuration "BlockPublicAcls=true,IgnorePublicAcls=true,BlockPublicPolicy=true,RestrictPublicBuckets=true"

  aws s3api put-bucket-encryption \
    --bucket $bucket_name \
    --profile $profile_name \
    --region $region_name \
    --server-side-encryption-configuration '{"Rules": [{"ApplyServerSideEncryptionByDefault": {"SSEAlgorithm": "AES256"}}]}'

  sed -i "" "s/arn:aws:s3:::bucket_placeholder/arn:aws:s3:::$bucket_name/g" "setup/$profile_name.json"

  aws s3api put-bucket-policy --bucket $bucket_name --profile $profile_name  --region $region_name --policy "file://setup/policy.json"

  aws dynamodb create-table --table-name terraform-locks --key-schema AttributeName=LockID,KeyType=HASH \
    --attribute-definitions AttributeName=LockID,AttributeType=S \
    --sse-specification Enabled=False \
    --provisioned-throughput ReadCapacityUnits=5,WriteCapacityUnits=5 \
    --profile $profile_name \
    --region $region_name

else
  echo "You are missing requirements.  bucket_name, profile_name,  and region_name \n  ./setup/create_terraform_state_storage.sh project-tf-east project_lz_prod us-east-1"
fi
