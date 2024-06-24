FROM mcr.microsoft.com/dotnet/sdk:6.0 as build

WORKDIR /app

# Copy in the nuget file
COPY NuGet.Config .

# Copy the projects into the image and restore separate from the rest of the
# code, as this allows us to only have to regenerate these layers if dependencies
# change.
COPY src/RDM.Data.ImageViewer/*.csproj ./RDM.Data.ImageViewer/
COPY src/RDM.Data.ImageViewer.SqlServer/*.csproj ./RDM.Data.ImageViewer.SqlServer/
COPY src/RDM.DataTransferObjects.ImageViewerAPI/*.csproj ./RDM.DataTransferObjects.ImageViewerAPI/
COPY src/RDM.Maps.ImageViewerAPI/*.csproj ./RDM.Maps.ImageViewerAPI/
COPY src/RDM.Messaging.ImageViewerAPI/*.csproj ./RDM.Messaging.ImageViewerAPI/
COPY src/RDM.Models.ImageViewerAPI/*.csproj ./RDM.Models.ImageViewerAPI/
COPY src/RDM.Services.ImageViewerAPI/*.csproj ./RDM.Services.ImageViewerAPI/
COPY src/RDM.Services.ImageViewerAPI.Mock/*.csproj ./RDM.Services.ImageViewerAPI.Mock/
COPY src/RDM.Webservice.ImageViewerAPI/*.csproj ./RDM.Webservice.ImageViewerAPI/


RUN dotnet restore RDM.Webservice.ImageViewerAPI/

# Copy in all the files, so we can actually build them
COPY src/RDM.Data.ImageViewer/ ./RDM.Data.ImageViewer/
COPY src/RDM.Data.ImageViewer.SqlServer/ ./RDM.Data.ImageViewer.SqlServer/
COPY src/RDM.DataTransferObjects.ImageViewerAPI/ ./RDM.DataTransferObjects.ImageViewerAPI/
COPY src/RDM.Maps.ImageViewerAPI/ ./RDM.Maps.ImageViewerAPI/
COPY src/RDM.Messaging.ImageViewerAPI/ ./RDM.Messaging.ImageViewerAPI/
COPY src/RDM.Models.ImageViewerAPI/ ./RDM.Models.ImageViewerAPI/
COPY src/RDM.Services.ImageViewerAPI/ ./RDM.Services.ImageViewerAPI/
COPY src/RDM.Services.ImageViewerAPI.Mock/ ./RDM.Services.ImageViewerAPI.Mock/
COPY src/RDM.Webservice.ImageViewerAPI/ ./RDM.Webservice.ImageViewerAPI/

# Build it
WORKDIR /app/RDM.Webservice.ImageViewerAPI
RUN dotnet publish -c Release -o out

WORKDIR /app/RDM.Webservice.ImageViewerAPI/out

# Strip executable and writable flags from all files, and make sure that all
# directories have the executable flag set everywhere for everyone
RUN find . -type f | xargs chmod 0644 && \
  find . -type d | xargs chmod 0755

# Remove the appsettings file and replace it with the right file
RUN rm -f appsettings.json values.* && \
    mv appsettings.docker.json appsettings.json

# Build our runtime image.
FROM mcr.microsoft.com/dotnet/aspnet:6.0

# Expose our Prometheus port
EXPOSE 9342

WORKDIR /app
COPY --from=build /app/RDM.Webservice.ImageViewerAPI/out/ ./

# Run as nobody by default
USER nobody:nobody

CMD ["dotnet", "/app/RDM.Webservice.ImageViewerAPI.dll"]
