FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Lab-11-Ander-Bustamante.Domain/Lab-11-Ander-Bustamante.Domain.csproj Lab-11-Ander-Bustamante.Domain/
COPY Lab-11-Ander-Bustamante.Application/Lab-11-Ander-Bustamante.Application.csproj Lab-11-Ander-Bustamante.Application/
COPY Lab-11-Ander-Bustamante.Infrastructure/Lab-11-Ander-Bustamante.Infrastructure.csproj Lab-11-Ander-Bustamante.Infrastructure/
COPY Lab-11-Ander-Bustamante.Api/Lab-11-Ander-Bustamante.Api.csproj Lab-11-Ander-Bustamante.Api/
RUN dotnet restore Lab-11-Ander-Bustamante.Api/Lab-11-Ander-Bustamante.Api.csproj

COPY . .
RUN dotnet publish Lab-11-Ander-Bustamante.Api/Lab-11-Ander-Bustamante.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

CMD ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT:-8080} exec dotnet Lab-11-Ander-Bustamante.Api.dll"]
