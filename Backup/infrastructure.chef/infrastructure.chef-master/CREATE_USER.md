# Create User
In order to be able to upload cookbooks and interact with the Chef Server you need to have a user created.  In order to create a new user the following has to be done.

1. You need to connect to the Chef Server at this address 10.194.90.99.
2. Once on the server you need to switch over to root.  `sudo su -`
3. As root you can run the chef server cli to create the user as follows. It will prompt for a password.
```bash
chef-server-ctl user-create '<first_last_name>' first_name last_name email --filename /home/pem_file_name.pem -p
```
4. Once you have created a user you need to add the user to the org you want them added too.  That is accomplished by running the following.
```bash
chef-server-ctl org-user-add <org_name> <username>
```
5. Give the pem file and credentials to new chef user.  Remind them to reset their password.
