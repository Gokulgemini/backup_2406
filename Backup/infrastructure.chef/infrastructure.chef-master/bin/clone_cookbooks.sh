#!/usr/bin/env bash
# This script should run from the root directory of the main chef repo.  This is a quick
# helper script to pull down any repos needed from bitbucket as we are breaking our
# cookbooks to multiple repos.  You should run this periodically to pull down any new
# cookbooks that have been added.  It relies on us keeping the cookbooks file in site-cookbooks
# updated with the cookbooks we have in bitbucket.

cd site-cookbooks/
cookbook_file="cookbooks"
git=`which git`
while IFS= read -r line
do
  echo "Trying to clone $line"
  if [ ! -d $line ]
  then
    echo "Cloning $line"
    $git clone ssh://git@bitbucket.deluxe.com:7999/echecks/infrastructure.chef.$line.git $line
    echo "site-cookbooks/$line" >> ../.gitignore
  else
    echo "Repo already cloned.  Consider doing a pull."
  fi
done < "$cookbook_file"
