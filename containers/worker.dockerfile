FROM mcr.microsoft.com/dotnet/sdk:9.0

WORKDIR /app

COPY deploy/worker/. ./

EXPOSE 8080

ENTRYPOINT ["dotnet", "EtherGizmos.Shipyard.Worker.dll"]
