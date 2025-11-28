FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore IAMService.sln
RUN dotnet publish IAMService.sln -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5001
ENTRYPOINT ["dotnet", "IAMService.Presentation.dll"]
