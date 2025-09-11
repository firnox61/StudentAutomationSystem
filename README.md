# 🎓 Student Automation System

Modern, ölçeklenebilir bir öğrenci yönetim sistemi. Clean Architecture prensiplerine uygun olarak geliştirilmiş, .NET 9.0 tabanlı RESTful API.

## 📋 İçindekiler

- [Proje Hakkında](#-proje-hakkında)
- [Teknoloji Stack](#-teknoloji-stack)
- [Mimari Yapı](#-mimari-yapı)
- [Proje Yapısı](#-proje-yapısı)
- [Kurulum](#-kurulum)
- [Kullanım](#-kullanım)
- [API Dokümantasyonu](#-api-dokümantasyonu)
- [Test](#-test)
- [Katkıda Bulunma](#-katkıda-bulunma)

## 🎯 Proje Hakkında

Student Automation System, eğitim kurumlarının öğrenci, öğretmen, ders ve not yönetimini dijitalleştiren kapsamlı bir sistemdir. Sistem, modern yazılım geliştirme pratiklerini takip ederek yüksek kaliteli, bakım yapılabilir ve test edilebilir bir kod tabanı sunar.

### Temel Özellikler

- 👨‍🎓 **Öğrenci Yönetimi** - Kayıt, güncelleme, listeleme
- 👨‍🏫 **Öğretmen Yönetimi** - Öğretmen bilgileri ve ders atamaları
- 📚 **Ders Yönetimi** - Ders oluşturma ve düzenleme
- 📊 **Not Sistemi** - Not girişi ve takibi
- 📅 **Devam Takibi** - Öğrenci devam durumu
- 💬 **Geri Bildirim** - Öğrenci değerlendirmeleri
- 🔐 **JWT Authentication** - Güvenli kimlik doğrulama
- 🛡️ **Role-based Authorization** - Yetkilendirme sistemi
- 📋 **Audit Logging** - İşlem kayıtları
- ⚡ **Performance Monitoring** - Performans takibi
- 📊 **Caching** - Redis/Memory cache desteği

## 🛠️ Teknoloji Stack

### Backend
- **.NET 9.0** - Framework
- **C#** - Programlama dili
- **ASP.NET Core Web API** - RESTful API
- **Entity Framework Core** - ORM
- **PostgreSQL** - Veritabanı
- **AutoMapper** - Object mapping
- **FluentValidation** - Doğrulama
- **Serilog** - Logging
- **Autofac** - Dependency Injection
- **Castle DynamicProxy** - AOP (Aspect-Oriented Programming)

### Security & Authentication
- **JWT (JSON Web Tokens)** - Token tabanlı kimlik doğrulama
- **BCrypt** - Şifre hash'leme
- **Custom Security Aspects** - Güvenlik katmanları

### Development & Testing
- **xUnit** - Test framework
- **Moq** - Mocking library
- **FluentAssertions** - Test assertions
- **Swagger/OpenAPI** - API dokümantasyonu

### Architecture Patterns
- **Clean Architecture** - Katmanlı mimari
- **Repository Pattern** - Veri erişim katmanı
- **CQRS** - Command Query Responsibility Segregation
- **AOP** - Aspect-Oriented Programming
- **Result Pattern** - Sonuç yönetimi

## 🏗️ Mimari Yapı

Proje Clean Architecture prensiplerine uygun olarak 5 ana katmandan oluşmaktadır:

```
┌─────────────────────────────────────────────────┐
│                 WebAPI Layer                    │
│           (Controllers, Middleware)             │
└─────────────────────────────────────────────────┘
                          │
┌─────────────────────────────────────────────────┐
│              Application Layer                  │
│        (Services, DTOs, Interfaces)            │
└─────────────────────────────────────────────────┘
                          │
┌─────────────────────────────────────────────────┐
│                Core Layer                       │
│        (Aspects, Utilities, Results)           │
└─────────────────────────────────────────────────┘
                          │
┌─────────────────────────────────────────────────┐
│               Domain Layer                      │
│             (Entities, Security)                │
└─────────────────────────────────────────────────┘
                          │
┌─────────────────────────────────────────────────┐
│            Infrastructure Layer                 │
│      (Database, Repositories, Security)        │
└─────────────────────────────────────────────────┘
```

### Katman Sorumlulukları

- **WebAPI**: HTTP istekleri, routing, middleware'ler
- **Application**: İş mantığı, servisler, DTO'lar, validasyonlar  
- **Core**: Cross-cutting concerns, AOP aspects, utilities
- **Domain**: Entity'ler, domain modelleri, interfaces
- **Infrastructure**: Veritabanı, external services, implementasyonlar

## 📁 Proje Yapısı

```
StudentAutomation/
├── 🌐 StudentAutomation.WebAPI/          # Presentation Layer
│   ├── Controllers/                      # API Controllers
│   ├── Middlewares/                      # Custom middlewares
│   ├── DependencyInjection/             # IoC configurations
│   ├── getClaim/                        # User claim management
│   └── logs/                            # Application logs
│
├── 🧠 StudentAutomation.Application/     # Application Layer
│   ├── Services/Managers/               # Business logic services
│   ├── DTOs/                           # Data Transfer Objects
│   ├── Interfaces/Services/            # Service contracts
│   ├── Repositories/                   # Repository interfaces
│   ├── MappingProfiles/                # AutoMapper profiles
│   ├── Validators/                     # FluentValidation rules
│   └── Abstraction/                    # Common abstractions
│
├── ⚙️ StudentAutomation.Core/           # Core Layer
│   ├── Aspects/                        # AOP aspects
│   │   ├── Caching/                    # Cache management
│   │   ├── Logging/                    # Logging aspects
│   │   ├── Security/                   # Security aspects
│   │   ├── Validation/                 # Validation aspects
│   │   └── Performance/                # Performance monitoring
│   ├── Utilities/                      # Helper utilities
│   │   ├── IoC/                       # IoC container
│   │   ├── Results/                   # Result pattern
│   │   └── Validation/                # Validation tools
│   ├── Extensions/                     # Extension methods
│   └── Services/                      # Core services
│
├── 🏛️ StudentAutomation.Domain/         # Domain Layer
│   ├── Entities/                       # Domain entities
│   │   ├── Student.cs                 # Öğrenci entity
│   │   ├── Teacher.cs                 # Öğretmen entity
│   │   ├── Course.cs                  # Ders entity
│   │   ├── Grade.cs                   # Not entity
│   │   ├── Attendance.cs              # Devam entity
│   │   ├── Enrollment.cs              # Kayıt entity
│   │   ├── StudentFeedback.cs         # Geri bildirim entity
│   │   ├── User.cs                    # Kullanıcı entity
│   │   ├── OperationClaim.cs          # Yetki entity
│   │   ├── UserOperationClaim.cs      # Kullanıcı-Yetki relation
│   │   └── AuditLog.cs               # Audit log entity
│   └── Security/                      # Domain security
│
├── 🗄️ StudentAutomation.Infrastructure/ # Infrastructure Layer
│   ├── Persistence/                    # Data persistence
│   │   ├── Context/                   # Database context
│   │   └── Repositories/EntityFramework/ # EF implementations
│   ├── Security/                      # Security implementations
│   │   ├── Jwt/                      # JWT token management
│   │   ├── Hashing/                  # Password hashing
│   │   └── Encryption/               # Encryption utilities
│   ├── Migrations/                    # Database migrations
│   └── Aspects/Services/             # Infrastructure services
│
└── 🧪 StudentAutomation.Tests.Unit/     # Unit Tests
    ├── Common/                        # Test base classes
    ├── CourseManager_Tests.cs         # Ders yönetimi testleri
    ├── StudentManager_Add_Tests.cs    # Öğrenci ekleme testleri
    ├── TeacherManager_Add_Tests.cs    # Öğretmen ekleme testleri
    └── UserManagerTests.cs           # Kullanıcı yönetimi testleri
```

## 🚀 Kurulum

### Gereksinimler

- .NET 9.0 SDK
- PostgreSQL 13+
- Visual Studio 2022 / VS Code
- Git

### Adım Adım Kurulum

1. **Repository'yi klonlayın**
```bash
git clone https://github.com/yourusername/StudentAutomation.git
cd StudentAutomation
```

2. **Veritabanı bağlantı dizesini yapılandırın**
```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=StudentAutomationDB;Username=postgres;Password=yourpassword"
  }
}
```

3. **NuGet paketlerini yükleyin**
```bash
dotnet restore
```

4. **Veritabanını oluşturun**
```bash
cd StudentAutomation.Infrastructure
dotnet ef database update
```

5. **Uygulamayı çalıştırın**
```bash
cd ../StudentAutomation.WebAPI
dotnet run
```

6. **Swagger UI'ya erişin**
```
https://localhost:7001/swagger
```

## 📖 Kullanım

### Authentication

Sisteme erişim için önce authentication endpoint'ini kullanarak token almalısınız:

```bash
POST /api/auth/login
{
  "email": "admin@example.com",
  "password": "password123"
}
```

Dönen token'ı Authorization header'ında kullanın:
```bash
Authorization: Bearer {your-jwt-token}
```

### Temel API Kullanımı

#### Öğrenci Ekleme
```bash
POST /api/students
{
  "firstName": "Ahmet",
  "lastName": "Yılmaz",
  "email": "ahmet.yilmaz@email.com",
  "phoneNumber": "05551234567",
  "dateOfBirth": "2000-01-15"
}
```

#### Ders Oluşturma  
```bash
POST /api/courses
{
  "courseName": "Matematik",
  "courseCode": "MAT101",
  "credits": 4,
  "teacherId": 1
}
```

#### Not Girişi
```bash
POST /api/grades
{
  "studentId": 1,
  "courseId": 1,
  "midtermGrade": 85.5,
  "finalGrade": 92.0,
  "letterGrade": "AA"
}
```

## 📚 API Dokümantasyonu

API dokümantasyonu Swagger UI üzerinden erişilebilir. Uygulama çalıştıktan sonra:

```
https://localhost:7001/swagger
```

### Ana Endpoint'ler

| Endpoint | Açıklama |
|----------|----------|
| `POST /api/auth/login` | Kullanıcı girişi |
| `POST /api/auth/register` | Kullanıcı kaydı |
| `GET /api/students` | Öğrenci listesi |
| `POST /api/students` | Yeni öğrenci |
| `GET /api/courses` | Ders listesi |
| `POST /api/grades` | Not girişi |
| `GET /api/attendances` | Devam kayıtları |

## 🧪 Test

### Unit Test Çalıştırma

```bash
cd StudentAutomation.Tests.Unit
dotnet test
```

### Test Coverage

```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Test Yapısı

- **CourseManager_Tests**: Ders yönetimi testleri
- **StudentManager_Add_Tests**: Öğrenci ekleme işlemleri testleri  
- **TeacherManager_Add_Tests**: Öğretmen ekleme işlemleri testleri
- **UserManagerTests**: Kullanıcı yönetimi testleri

## 🔧 Yapılandırma

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Your-PostgreSQL-Connection-String"
  },
  "TokenOptions": {
    "Audience": "student-automation",
    "Issuer": "student-automation",
    "AccessTokenExpiration": 60,
    "SecurityKey": "your-super-secret-key-here"
  },
  "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "path": "logs/log-.txt",
          "rollingInterval": "Day"
        }
      }
    ]
  }
}
```

## 🤝 Katkıda Bulunma

1. Fork'layın
2. Feature branch oluşturun (`git checkout -b feature/yeni-ozellik`)
3. Değişikliklerinizi commit'leyin (`git commit -am 'Yeni özellik eklendi'`)
4. Branch'inizi push'layın (`git push origin feature/yeni-ozellik`)
5. Pull Request oluşturun

### Kod Standartları

- Clean Architecture prensiplerine uyun
- SOLID prensiplerine dikkat edin
- Unit test yazın
- XML dokümantasyonu ekleyin
- Anlamlı commit mesajları kullanın

## 📄 Lisans

Bu proje MIT lisansı altında lisanslanmıştır. Detaylar için [LICENSE](LICENSE) dosyasına bakın.

## 📞 İletişim

- **Geliştirici**: [Your Name]
- **Email**: [your.email@example.com]
- **LinkedIn**: [Your LinkedIn Profile]
- **GitHub**: [Your GitHub Profile]

---

**Not**: Bu README dosyası projenin mevcut durumunu yansıtmaktadır. Yeni özellikler eklendikçe güncellenmektedir.
