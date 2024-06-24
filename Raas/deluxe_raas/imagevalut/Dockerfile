# GRPC stuff won't build on alpine due to libc differences
# So, as a working around docker build issue until open-cv issue is resolved I commented out the following line
# FROM docker-local.artifactory.deluxe.com/opencv/dotnet/core/sdk:3.1 as build
FROM mcr.microsoft.com/dotnet/sdk:6.0 as build

WORKDIR /app

# Copy in the nuget file
COPY NuGet.Config .

# Copy the projects into the image and restore separate from the rest of the
# code, as this allows us to only have to regenerate these layers if dependencies
# change.
COPY src/RDM.Client.ImageVault/*.csproj ./RDM.Client.ImageVault/
COPY src/RDM.Data.ImageVault/*.csproj ./RDM.Data.ImageVault/
COPY src/RDM.Data.ImageVault.SqlServer/*.csproj ./RDM.Data.ImageVault.SqlServer/
COPY src/RDM.GRPC.ImageVault/*.csproj ./RDM.GRPC.ImageVault/
COPY src/RDM.Imaging/*.csproj ./RDM.Imaging/
COPY src/RDM.Messaging.ImageVault/*.csproj ./RDM.Messaging.ImageVault/
COPY src/RDM.Model.ImageVault/*.csproj ./RDM.Model.ImageVault/
COPY src/RDM.Service.ImageVault/*.csproj ./RDM.Service.ImageVault/
COPY src/RDM.Services.ImageVault/*.csproj ./RDM.Services.ImageVault/

RUN dotnet restore RDM.Service.ImageVault/

# Copy in all the files, so we can actually build them
COPY src/RDM.Client.ImageVault/ ./RDM.Client.ImageVault/
COPY src/RDM.Data.ImageVault/ ./RDM.Data.ImageVault/
COPY src/RDM.Data.ImageVault.SqlServer/ ./RDM.Data.ImageVault.SqlServer/
COPY src/RDM.GRPC.ImageVault/ ./RDM.GRPC.ImageVault/
COPY src/RDM.Imaging/ ./RDM.Imaging/
COPY src/RDM.Messaging.ImageVault/ ./RDM.Messaging.ImageVault/
COPY src/RDM.Model.ImageVault/ ./RDM.Model.ImageVault/
COPY src/RDM.Service.ImageVault/ ./RDM.Service.ImageVault/
COPY src/RDM.Services.ImageVault/ ./RDM.Services.ImageVault/

# Build it
WORKDIR /app/RDM.Service.ImageVault
RUN dotnet publish -c Release -o out

# Strip executable and writable flags from everything
WORKDIR /app/RDM.Service.ImageVault/out
RUN chmod a-wX *

# Remove the appsettings file so it has to be there later
RUN rm -f appsettings.*

# Build our runtime image.
# So, as a working around docker build issue until open-cv issue is resolved I commented out the following line 
# FROM docker-local.artifactory.deluxe.com/opencv/dotnet/core/runtime:3.1
FROM mcr.microsoft.com/dotnet/aspnet:6.0

# Expose our Prometheus port
EXPOSE 9342

WORKDIR /app
COPY --from=build /app/RDM.Service.ImageVault/out/ ./
RUN mkdir /config \
    && ln -s /config/appsettings.json /app/appsettings.json

# Run as nobody by default
USER nobody:nobody

CMD ["dotnet", "/app/RDM.Service.ImageVault.dll"]
