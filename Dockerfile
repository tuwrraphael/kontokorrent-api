FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /App

COPY Kontokorrent ./

RUN dotnet restore
RUN dotnet publish -c Release -o './out' 


FROM mcr.microsoft.com/dotnet/aspnet:3.1
WORKDIR /App
COPY --from=build /App/out .
ENTRYPOINT ["dotnet", "Kontokorrent.dll", "--launch-profile", "Docker"]