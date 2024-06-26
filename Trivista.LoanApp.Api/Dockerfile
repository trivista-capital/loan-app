#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Trivista.LoanApp.Api/Trivista.LoanApp.Api.csproj", "Trivista.LoanApp.Api/"]
COPY ["Trivista.LoanApp.ApplicationCore/Trivista.LoanApp.ApplicationCore.csproj", "Trivista.LoanApp.ApplicationCore/"]
RUN dotnet restore "./Trivista.LoanApp.Api/./Trivista.LoanApp.Api.csproj"
COPY . .
WORKDIR "/src/Trivista.LoanApp.Api"
RUN dotnet build "./Trivista.LoanApp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Trivista.LoanApp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Trivista.LoanApp.Api.dll"]