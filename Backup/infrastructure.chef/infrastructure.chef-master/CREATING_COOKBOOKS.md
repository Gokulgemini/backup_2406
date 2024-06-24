# Creating New Cookbooks
The following steps should be followed when we are creating a new cookbook.
1. In the root of the chef repo we will run the generate command.
```bash
$ chef exec chef generate cookbook site-cookbooks/<cookbook_name> -m 'devops@deluxe.com' -C 'Deluxe DevOps' -b
```
2. At this point this cookbook should not be tracked in the chef repo but instead in its own.  So we will add the cookbook to the gitignore.
```bash
$ echo "site-cookbooks/<cookbook_name>" >> .gitignore
```
3. Now we need to add this new cookbook to the cookbooks file so we can pull it down in the future by using our utility scripts.
```bash
$ echo "<cookbook_name>" >> site-cookbooks/cookbooks
```
4. At this point you should create a repo in our BitBucket account. The project in BitBucket is ECHECKS and its located at https://bitbucket.deluxe.com/projects/ECHECKS. The naming convention to follow is `infrastructure.chef.<cookbook_name>`.
