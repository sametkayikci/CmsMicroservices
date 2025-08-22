# 📂 CmsMicroservices

Bu proje, İçerik Yönetim Sistemi (CMS) için geliştirilmiş iki mikroservisli bir mimari sunar: **Users** ve **Contents**. Amaç, modüler, temiz, test edilebilir, güvenli ve performanslı bir yapı kurmaktır.

---

## ✅ Teknolojiler

- .NET 9
- Entity Framework Core 9
- PostgreSQL
- Redis
- xUnit
- Refit
- FluentValidation
- SeriLog
- Docker / Docker Compose
- Clean Architecture prensipleri

---

## 🏡 Katmanlar & Proje Yapısı

```
CmsMicroservices/
├─ src/
│  ├─ BuildingBlocks/
│  │  ├─ Cms.Shared/                # Ortak cache, logging, security, extension servisleri
│  ├─ Services/
│  │  ├─ Users/
│  │  │  └─ Users.Api/             # User mikroservisi
│  │  └─ Contents/
│  │     └─ Contents.Api/         # Content mikroservisi
├─ tests/
│  ├─ Users/
│  │  └─ Users.Tests/
│  └─ Contents/
│     └─ Contents.Tests/
├─ docker-compose.yml
└─ README.md
```

---

## 🔗 Mikroservisler Arası İletişim

### 💡 Kullanıcı Silinince İçerikler de Silinsin

- **Contents.Api**, `DELETE /contents/by-user/{userId}` endpoint'ine sahiptir.
- **Users.Api**, `DeleteAsync` metodu içinde bu endpoint'i REST olarak çağırarak ilgili kullanıcının içeriklerini temizler.

```csharp
await _httpClient.DeleteAsync($"http://contents.api/contents/by-user/{userId}");
```

---

## 📊 Modüller

### Users.Api

- Kullanıcı CRUD
- POST /users
- GET /users
- GET /users/{id}
- PUT /users/{id}
- DELETE /users/{id} => İlgili kullanıcının içerikleri de silinir.

### Contents.Api

- İçerik CRUD
- POST /contents
- GET /contents
- GET /contents/{id}
- PUT /contents/{id}
- DELETE /contents/{id}
- DELETE /contents/by-user/{userId}

---

## ⚙️ Infrastructure

### RedisCacheService
- Idempotency Middleware için Redis kullanılır.
- `ICacheService` arayüzüyle sarmallanır.

### PostgreSQL
- EF Core `DbContext` kullanımıyla tablo ve index oluşturma

### Logging & Observability
- Serilog

### Exception Handling
- `ExceptionMiddleware` ile JSON şeklinde 500 response ve loglama

---

## 📖 ServiceCollection Extension'lar

**Cms.Shared.Extensions:**
- `AddApiLivenessChecks`
- `AddFixedWindowRateLimiting`
- `AddCustomSwagger`


**Users.Api & Contents.Api:**
- Her servis `AddXxxModule()` uzantı metodu ile servislerini tanımlar

---

## 📡 Unit Testler

xUnit + Moq kullanılmıştır. 

### ✨ Örnekler:

**ExceptionMiddlewareTests.cs**
```csharp
[Fact]
public async Task GivenThrowingDelegateWhenInvokeThenReturns500Json()
```

**UsersModuleRegistrationTests.cs**
```csharp
[Fact]
public void GivenConfigurationWhenAddUsersModuleThenRequiredServicesAreResolvable()
```

**InternalControllerTests.cs** (Contents)
```csharp
[Fact]
public async Task GivenCorrectKeyWhenDeleteByUserThenOkWithCount()
```

---

## 🛠 Docker

**docker-compose.yml:**
```yaml
services:
  postgres:
    image: postgres:latest
    environment:
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
      POSTGRES_DB: cms
    ports:
      - "5432:5432"

  redis:
    image: redis:latest
    ports:
      - "6379:6379"

  users.api:
    build:
      context: ./src/Services/Users/Users.Api
    depends_on:
      - postgres
      - redis

  contents.api:
    build:
      context: ./src/Services/Contents/Contents.Api
    depends_on:
      - postgres
      - redis
```



## 🌍 Kurulum

```bash
docker compose -f src/docker-compose.yml up -d
```

### Swagger:
- Users: `http://localhost:5001/swagger`
- Contents: `http://localhost:5002/swagger`

---

