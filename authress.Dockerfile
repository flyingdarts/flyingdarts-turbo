# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY packages/backend/dotnet/Flyingdarts.Authress.Token.Service/Flyingdarts.Authress.Token.Service.csproj packages/backend/dotnet/Flyingdarts.Authress.Token.Service/Flyingdarts.Authress.Token.Service.csproj

RUN dotnet restore packages/backend/dotnet/Flyingdarts.Authress.Token.Service/Flyingdarts.Authress.Token.Service.csproj

# Copy source and publish
COPY packages/backend/dotnet/Flyingdarts.Authress.Token.Service/ packages/backend/dotnet/Flyingdarts.Authress.Token.Service/

RUN dotnet publish packages/backend/dotnet/Flyingdarts.Authress.Token.Service/Flyingdarts.Authress.Token.Service.csproj -c Release -o /app/out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

ENV ASPNETCORE_URLS=http://0.0.0.0:8080

EXPOSE 8080

COPY --from=build /app/out ./

ENTRYPOINT ["dotnet", "Flyingdarts.Authress.Token.Service.dll"]
