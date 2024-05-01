FROM mcr.microsoft.com/dotnet/sdk:8.0
FROM nginx:1.25.5

COPY nginx.conf /etc/nginx/conf.d/default.conf
WORKDIR /app

COPY ./bin/Release/net8.0/publish /KiraBot
CMD ["dotnet" ""]
