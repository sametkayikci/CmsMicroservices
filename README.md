# CmsMicroservices

Modern .NET 9 tabanlı **mikroservis örneği**: Users ve Contents servisleri, ortak `Cms.Shared` kütüphanesi, Docker Compose (PostgreSQL + Redis), birim testleri ve sağlık kontrolü (liveness).

## İçerik
- [Mimari Genel Bakış](#mimari-genel-bakış)
- [Klasör Yapısı](#klasör-yapısı)
- [Gereksinimler](#gereksinimler)
- [Hızlı Çalıştırma](#hızlı-çalıştırma)
- [Servisleri Çalıştırma (dotnet run)](#servisleri-çalıştırma-dotnet-run)
- [Docker Compose ile Çalıştırma](#docker-compose-ile-çalıştırma)
- [Sağlık Kontrolleri](#sağlık-kontrolleri)
- [Testler](#testler)
- [Yapı Taşları](#yapı-taşları)
- [Faydalı Komutlar](#faydalı-komutlar)

---

## Mimari Genel Bakış

- **Users.Api**: Kullanıcı CRUD.
- **Contents.Api**: İçerik CRUD (kullanıcı doğrulama ve kullanıcı silinince içerik temizleme senaryoları).
- **Cms.Shared**: Ortak soyutlamalar, middleware, observability, security, refit client’lar, health check yardımcıları vb.
- **tests/**: İki servis için ayrı birim test projeleri.

---

## Klasör Yapısı

> Aşağıdaki yapı, ekran görüntüsündeki mevcut proje düzenine göre günceldir.

```
CmsMicroservices.sln
├─ src/
│  ├─ BuildingBlocks/
│  │  └─ Cms.Shared/
│  │     ├─ Abstractions/
│  │     ├─ Caching/
│  │     ├─ Contracts/
│  │     ├─ Extensions/
│  │     ├─ HealthChecks/
│  │     ├─ Middleware/
│  │     ├─ Observability/
│  │     ├─ RefitClients/
│  │     ├─ Security/
│  │     └─ GlobalUsings.cs
│  └─ Services/
│     ├─ Contents/
│     │  └─ Contents.Api/
│     │     ├─ Extensions/
│     │     ├─ Features/
│     │     │  └─ Contents/
│     │     │     ├─ Controllers/
│     │     │     ├─ Data/
│     │     │     ├─ Entities/
│     │     │     ├─ Repositories/
│     │     │     ├─ Services/
│     │     │     └─ Validators/
│     │     ├─ appsettings.json
│     │     ├─ appsettings.Development.json
│     │     ├─ Contents.Api.http
│     │     ├─ Dockerfile
│     │     ├─ GlobalUsings.cs
│     │     └─ Program.cs
│     └─ Users/
│        └─ Users.Api/
│           ├─ Extensions/
│           ├─ Features/
│           │  └─ Users/
│           │     ├─ Controllers/
│           │     ├─ Data/
│           │     ├─ Entities/
│           │     ├─ Repositories/
│           │     ├─ Services/
│           │     └─ Validators/
│           ├─ appsettings.json
│           ├─ appsettings.Development.json
│           ├─ Dockerfile
│           ├─ GlobalUsings.cs
│           └─ Program.cs
├─ tests/
│  ├─ Contents/
│  │  └─ Contents.Test/
│  │     ├─ ContentRepositoryTests.cs
│  │     ├─ ContentsControllerTests.cs
│  │     ├─ ContentServiceTests.cs
│  │     ├─ ContentsModuleRegistrationTests.cs
│  │     ├─ ContentValidatorTests.cs
│  │     ├─ InternalControllerTests.cs
│  │     └─ GlobalUsings.cs
│  └─ Users/
│     └─ Users.Tests/
│        ├─ ExceptionHandlingMiddlewareTests.cs
│        ├─ RedisCacheServiceTests.cs
│        ├─ SecurityHeadersMiddlewareTests.cs
│        ├─ UserRepositoryTests.cs
│        ├─ UsersControllerTests.cs
│        ├─ UserServiceTests.cs
│        ├─ UsersModuleRegistrationTests.cs
│        └─ GlobalUsings.cs
├─ docker-compose.yml
└─ README.md
```

---

## Gereksinimler

- **.NET 9 SDK**
- **Docker Desktop** (Compose etkin)
- (Opsiyonel) **curl**

---

## Hızlı Çalıştırma

```bash
# 1) Bağımlılıklar
dotnet restore

# 2) Derleme
dotnet build -c Debug

# 3) Veritabanı ve Redis'i ayağa kaldır
docker compose up -d postgres redis
```

> Compose dosyasındaki varsayılan portlar: **PostgreSQL 5433→5432**, **Redis 6379→6379**

---

## Servisleri Çalıştırma (dotnet run)

> Host üzerinden doğrudan çalıştırma (Docker’da yalnızca Postgres & Redis açıkken).

```bash
# Users.Api  → http://localhost:5001
dotnet run --project src/Services/Users/Users.Api/Users.Api.csproj --urls http://0.0.0.0:5001

# Contents.Api → http://localhost:5002
dotnet run --project src/Services/Contents/Contents.Api/Contents.Api.csproj --urls http://0.0.0.0:5002
```

**Ortam Değişkenleri (gerekirse örnek):**
```bash
# Users.Api
ASPNETCORE_ENVIRONMENT=Development ConnectionStrings__Postgres="Host=localhost;Port=5433;Database=users_db;Username=postgres;Password=postgres" Redis__Connection="localhost:6379" Services__ContentsBaseUrl="http://localhost:5002" Internal__Key="very-secret-internal-key" dotnet run --project src/Services/Users/Users.Api/Users.Api.csproj --urls http://0.0.0.0:5001
```

```bash
# Contents.Api
ASPNETCORE_ENVIRONMENT=Development ConnectionStrings__Postgres="Host=localhost;Port=5433;Database=contents_db;Username=postgres;Password=postgres" Redis__Connection="localhost:6379" Services__UsersBaseUrl="http://localhost:5001" Internal__Key="very-secret-internal-key" dotnet run --project src/Services/Contents/Contents.Api/Contents.Api.csproj --urls http://0.0.0.0:5002
```

---

## Docker Compose ile Çalıştırma

> Tamamını container olarak kaldırma:

```bash
docker compose up -d --build
# veya yalnız API'ler:
docker compose up -d usersapi contentsapi
```

- Users.Api: `http://localhost:5001`
- Contents.Api: `http://localhost:5002`

**Compose içi servis adresleri:**
- `users.api:8080`
- `contents.api:8080`

---

## Sağlık Kontrolleri

Her servis kendi liveness endpoint’ini expose eder:

```
GET /health
Users   → http://localhost:5001/health
Contents→ http://localhost:5002/health
```

**Örnek doğrulama:**
```bash
curl -f http://localhost:5001/health
curl -f http://localhost:5002/health
```

> Compose’ta container healthcheck (önerilir):
```yaml
healthcheck:
  test: ["CMD-SHELL", "curl -f http://localhost:8080/health || exit 1"]
  interval: 10s
  timeout: 3s
  retries: 10
  start_period: 10s
```

---

## Testler

Tüm çözüm için:
```bash
dotnet test
```

Tekil proje bazında:
```bash
dotnet test tests/Users/Users.Tests/Users.Tests.csproj
dotnet test tests/Contents/Contents.Test/Contents.Test.csproj
```

**Kod örtüsü (opsiyonel):**
```bash
dotnet test -p:CollectCoverage=true -p:CoverletOutputFormat=opencover
```

---

## Yapı Taşları

- **Cms.Shared**
  - `Abstractions/` – ortak arayüzler (örn. `IDateTime`, `ICacheService`)
  - `Caching/` – Redis cache servisi ve testleri
  - `Middleware/` – Exception & Security Headers vb.
  - `Observability/` – Serilog, OpenTelemetry yardımcıları
  - `RefitClients/` – servisler arası HTTP client arayüzleri
  - `HealthChecks/` – URL ping tabanlı liveness check (paketsiz)
  - `Extensions/` – IoC ve ASP.NET Core uzantıları

- **Services/**
  - `Users.Api` & `Contents.Api` – Feature tabanlı (vertical slice) klasörleşme
  - `Features/*/Controllers` – REST uçları
  - `Features/*/Data` – EF Core DbContext, konfigürasyonlar
  - `Features/*/Repositories` – veri erişim
  - `Features/*/Services` – domain/service katmanı
  - `Features/*/Validators` – FluentValidation kuralları

- **tests/**
  - Controller, Service, Repository ve Middleware testleri

---

## Faydalı Komutlar

```bash
# Tüm NuGet paketlerini güncelle (yerel)
dotnet restore

# Çözümü temizle & tekrar derle
dotnet clean && dotnet build -c Release

# API’leri tek seferde stop/remove
docker compose down -v
```
