FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Directory.Build.props ./
COPY src/ConferenceHalls.Api/ConferenceHalls.Api.csproj src/ConferenceHalls.Api/
RUN dotnet restore src/ConferenceHalls.Api/ConferenceHalls.Api.csproj
COPY src/ConferenceHalls.Api/ src/ConferenceHalls.Api/
RUN dotnet publish src/ConferenceHalls.Api/ConferenceHalls.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
USER app
EXPOSE 8080
ENTRYPOINT ["dotnet", "ConferenceHalls.Api.dll"]
