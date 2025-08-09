FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Domain/Domain.csproj ./domain/
COPY webapi/webapi.csproj ./webapi/
COPY Infrastructure/Infrastructure.csproj ./Infrastructure/


RUN dotnet restore webapi/webapi.csproj


COPY . .
RUN dotnet publish webapi/webapi.csproj -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:3000

ENTRYPOINT ["dotnet", "webapi.dll"]
