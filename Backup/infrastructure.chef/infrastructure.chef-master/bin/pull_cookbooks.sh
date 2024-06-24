#!/usr/bin/env bash
# This script should run from the root directory of the main chef repo.  This is a quick
# helper script to pull down any changes for the cookbooks needed from bitbucket as we are breaking our
# cookbooks to multiple repos.  You should run this periodically to pull down any new
# changes to the cookbooks.  It relies on us keeping the cookbooks file in site-cookbooks
# updated with the cookbooks we have in bitbucket.

cd site-cookbooks/
cookbook_file="cookbooks"
git=`which git`
while IFS= read -r line
do
  echo "Attempting pull of updates on cookbook $line"
  if [ -d $line ]
  then
    echo "Pulling updates on $line"
    cd $line
    $git pull
  else
    echo "Repo is not cloned.  Consider doing a clone on $line."
  fi
done < "$cookbook_file"
