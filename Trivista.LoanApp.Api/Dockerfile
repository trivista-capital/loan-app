FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Trivista.LoanApp.Api/Trivista.LoanApp.Api.csproj", "Trivista.LoanApp.Api/"]
COPY ["Trivista.LoanApp.ApplicationCore/Trivista.LoanApp.ApplicationCore.csproj", "Trivista.LoanApp.ApplicationCore/"]
RUN dotnet restore "Trivista.LoanApp.Api/Trivista.LoanApp.Api.csproj"
COPY . .
WORKDIR "/src/Trivista.LoanApp.Api"
RUN dotnet build "Trivista.LoanApp.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Trivista.LoanApp.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Trivista.LoanApp.Api.dll"]
