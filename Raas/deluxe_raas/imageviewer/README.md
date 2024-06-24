# Using Docker
To build this image, use `docker build -t 'imageviewerapi:dev' .` while in the same directory as Dockerfile.

To run the image you've built, run `docker run -n 'imageviewerapi-dev' -d -v ``pwd``/src/RDM.Webservice.ImageViewerAPI/appsettings.Development.json:/config/appsettings.json imageviewerapi:dev` (Use a single backtick around `pwd`, not two).

More information can be found at https://deluxe.atlassian.net/wiki/spaces/DEVOPS/pages/1236468031/Readme+file+for+using+ITMS+API+Docker+images
