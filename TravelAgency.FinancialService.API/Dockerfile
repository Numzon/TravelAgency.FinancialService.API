#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["TravelAgency.FinancialService.API/TravelAgency.FinancialService.API.csproj", "TravelAgency.FinancialService.API/"]
RUN dotnet restore "TravelAgency.FinancialService.API/TravelAgency.FinancialService.API.csproj"
COPY . .
WORKDIR "/src/TravelAgency.FinancialService.API"
RUN dotnet build "TravelAgency.FinancialService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TravelAgency.FinancialService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TravelAgency.FinancialService.API.dll"]