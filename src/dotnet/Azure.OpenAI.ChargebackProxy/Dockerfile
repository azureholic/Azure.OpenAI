#BUILD
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY Azure.OpenAI.ChargebackProxy.ServiceDefaults/ ./Azure.OpenAI.ChargebackProxy.ServiceDefaults
COPY Azure.OpenAI.ChargebackProxy/ ./Azure.OpenAI.ChargebackProxy
RUN dotnet restore Azure.OpenAI.ChargebackProxy/Azure.OpenAI.ChargebackProxy.csproj
WORKDIR /src/Azure.OpenAI.ChargebackProxy
RUN dotnet build Azure.OpenAI.ChargebackProxy.csproj -c $BUILD_CONFIGURATION -o /app/build

#PUBLISH
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish Azure.OpenAI.ChargebackProxy.csproj -c $BUILD_CONFIGURATION -o /app/publish 

#FINAL
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet","./Azure.OpenAI.ChargebackProxy.dll"]