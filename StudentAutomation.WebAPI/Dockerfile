# 1. Build aşaması
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Sadece WebAPI ve referans projeleri
COPY StudentAutomation.Application/*.csproj StudentAutomation.Application/
COPY StudentAutomation.Core/*.csproj StudentAutomation.Core/
COPY StudentAutomation.Domain/*.csproj StudentAutomation.Domain/
COPY StudentAutomation.Infrastructure/*.csproj StudentAutomation.Infrastructure/
COPY StudentAutomation.WebAPI/*.csproj StudentAutomation.WebAPI/

# Restore WebAPI projesi
WORKDIR /src/StudentAutomation.WebAPI
RUN dotnet restore

# Tüm kaynak kodu kopyala
COPY StudentAutomation.Application/ ../StudentAutomation.Application/
COPY StudentAutomation.Core/ ../StudentAutomation.Core/
COPY StudentAutomation.Domain/ ../StudentAutomation.Domain/
COPY StudentAutomation.Infrastructure/ ../StudentAutomation.Infrastructure/
COPY StudentAutomation.WebAPI/ ./

# Publish
RUN dotnet publish -c Release -o /app/out

# 2. Runtime aşaması
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "StudentAutomation.WebAPI.dll"]
