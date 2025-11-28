FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore PatientService.sln
RUN dotnet publish PatientService.sln -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5002
ENTRYPOINT ["dotnet", "PatientService.Presentation.dll"]
