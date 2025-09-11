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
# Öğrenci Otomasyon Sistemi – Test ve Web Paneli

Projeyi test edebilmeniz için **hazır test verisi** sağlayamıyoruz çünkü sistemde kullanıcı parolaları **hash** ile saklanmaktadır. Testleri kendi ortamınızda yapmanız gerekmektedir.

## Test Kuralları
1. Sistemde her kullanıcı **tek bir role** sahiptir: ya **öğrenci** ya da **öğretmen**.
2. Kullanıcı kaydı yapmak için:
   - Web panelinde **Kayıt Ol** bölümünden yeni bir kullanıcı oluşturun.
3. Swagger tarafında oluşturduğunuz kullanıcıya **UserOperationClaims** ile gerekli yetki atamasını yapın.
4. Claim ataması dışındaki tüm işlemleri **web paneli üzerinden** gerçekleştirebilirsiniz.
5. API’nin geri kalan işlemleri çoğunlukla **backend tarafında** çalışmaktadır; web tarafında sadece erişebildiğiniz kısımları kullanabilirsiniz.

## Web Paneli Modülleri

### Öğrenciler
- Öğrenci ekleme
- Öğrenci detay görüntüleme
- Öğrenci güncelleme
- Öğrenci silme
- Filtreleme ve arama

### Öğretmenler
- Öğretmen ekleme
- Öğretmen detay görüntüleme
- Öğretmen güncelleme
- Öğretmen silme
- Filtreleme ve arama

### Kurslar
- Kurs ekleme
- Kurs detay görüntüleme
- Kurs güncelleme
- Kurs silme
- Filtreleme ve arama

### Yoklamalar
- Yoklama ekleme
- Yoklama detay görüntüleme
- Yoklama güncelleme
- Yoklama silme
- Filtreleme ve arama

### Notlar
- Not ekleme
- Not detay görüntüleme
- Not güncelleme
- Not silme

### Öğrenci Feedback
- Feedback ekleme
- Feedback detay görüntüleme
- Feedback güncelleme
- Feedback silme
- Filtreleme ve arama

### Ders Kayıtları
- Ders ekleme
- Ders silme
- Filtreleme ve arama

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
git clone https://github.com/firnox61/StudentAutomationSystem.git
cd StudentAutomation
```

2. **Veritabanı bağlantı dizesini yapılandırın**
```json
// appsettings.json
{
  "TokenOptions": {
    "Audience": "your@email.com",
    "Issuer": "your@email.com",
    "AccessTokenExpiration": 10,
    "SecurityKey": "mysupersecretkeymysupersecretkeymysupersecretkeymysupersecretkeymysupersecretkeymysupersecretkey"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=studentdb;Username=sa;Password=Password123*"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
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
http://localhost:5180/swagger/index.html
```
 **Web tarafına erişin**
http://localhost:5283/


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

- **Geliştirici**: [İsmail Enes Eroğlu]
- **Email**: [ismaileneseroglu@gmail.com]
- **LinkedIn**: [https://www.linkedin.com/in/ismail-enes-ero%C4%9Flu-4381a51ba/]
- **GitHub**: [https://github.com/firnox61]

---

**Not**: Bu README dosyası projenin mevcut durumunu yansıtmaktadır. Yeni özellikler eklendikçe güncellenmektedir.
<img width="1912" height="922" alt="1" src="https://github.com/user-attachments/assets/786c5b98-66b1-4766-9878-e0b6000629de" />
<img width="1260" height="697" alt="2" src="https://github.com/user-attachments/assets/402e2b78-9333-4939-b0e2-126ca6ddcf1b" />
<img width="1912" height="716" alt="3" src="https://github.com/user-attachments/assets/da9be180-25c0-4ee4-85e9-f59a3ebb93fc" />
<img width="1608" height="437" alt="4" src="https://github.com/user-attachments/assets/bff71d43-6e80-464f-9898-85a3750ab9c0" />
<img width="1897" height="642" alt="5" src="https://github.com/user-attachments/assets/a0ffc226-ac2d-42ed-83a9-f2c0d84c5479" />
<img width="1642" height="493" alt="6" src="https://github.com/user-attachments/assets/00225bb9-1c09-4e58-9471-e4c63f3041ff" />
<img width="1887" height="645" alt="7" src="https://github.com/user-attachments/assets/45983eaf-b79d-4f8a-8975-78cd86147864" />
<img width="1657" height="466" alt="8" src="https://github.com/user-attachments/assets/99b2828d-cd06-4e12-a4dc-144e7646b5f3" />
<img width="1876" height="701" alt="9" src="https://github.com/user-attachments/assets/b15fb64e-adec-4530-9984-8036b003f6c3" />
<img width="1903" height="541" alt="10" src="https://github.com/user-attachments/assets/eb9a9b9a-8433-4ac1-87aa-0348a1acc4c7" />
<img width="915" height="482" alt="11" src="https://github.com/user-attachments/assets/8d993613-bfce-4c75-995f-d2912756e75f" />

