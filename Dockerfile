FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

COPY tests3 /home/tests3
WORKDIR /home/tests3
RUN  dotnet build
RUN dotnet publish -c Release -o out 


# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
EXPOSE 5000
RUN apt update 
RUN apt install curl -y && apt install libgdiplus -y
WORKDIR /app
COPY --from=build /home/tests3/out .
#ENTRYPOINT ["dotnet", "LcmsWebApi.dll"]
ENTRYPOINT ["dotnet", "tests3.dll"]


