# ----------------------------
# 1️⃣ BUILD AŞAMASI (Multi-stage)
# ----------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Sadece csproj dosyalarını kopyalayıp restore yap (daha verimli cache)
COPY Core/UniCliqueBackend.Domain/UniCliqueBackend.Domain.csproj Core/UniCliqueBackend.Domain/
COPY Core/UniCliqueBackend.Application/UniCliqueBackend.Application.csproj Core/UniCliqueBackend.Application/
COPY Infrastructure/UniCliqueBackend.Persistence/UniCliqueBackend.Persistence.csproj Infrastructure/UniCliqueBackend.Persistence/
COPY UniCliqueBackendAPI/UniCliqueBackendAPI.csproj UniCliqueBackendAPI/

RUN dotnet restore UniCliqueBackendAPI/UniCliqueBackendAPI.csproj

# Tüm kaynak kodu kopyala ve publish et
COPY . .
RUN dotnet publish UniCliqueBackendAPI/UniCliqueBackendAPI.csproj -c Release -o /out


# ----------------------------
# 2️⃣ RUNTIME AŞAMASI
# ----------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Container port ve URL ayarı
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Bağlantı dizesi environment ile override edilebilir
# Örnek: ConnectionStrings__DefaultConnection
# ENV ConnectionStrings__DefaultConnection="Host=postgres;Port=5432;Database=uniclique_db;Username=postgres;Password=postgres"

COPY --from=build /out .
ENTRYPOINT ["dotnet", "UniCliqueBackendAPI.dll"]
