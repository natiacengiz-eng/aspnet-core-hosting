# ASP.NET Core Hosting Manager

Windows 10 ve Windows Server 2022 üzerinde ASP.NET Core uygulamalarını barındırmak için geliştirilmiş, profesyonel bir hosting çözümü.

## Özellikler

✅ **Çoklu Site Yönetimi** - Aynı anda birden fazla ASP.NET Core uygulaması çalıştırın
✅ **Windows Service Desteği** - Uygulamaları Windows servisi olarak çalıştırın
✅ **Otomatik Yeniden Başlatma** - Uygulama çökmesi durumunda otomatik başlatma
✅ **Sağlık Kontrolleri** - Real-time uygulama sağlığı izleme
✅ **SSL/HTTPS Desteği** - Güvenli bağlantılar için sertifika yönetimi
✅ **Kapsamlı Logging** - Detaylı log kayıtları ve hata izleme
✅ **REST API** - Uygulamalar üzerinde kontrol için API
✅ **Yönetim Paneli** - Web-tabanlı yönetim arayüzü

## Gereksinimler

- Windows 10 Pro/Enterprise veya Windows Server 2022
- .NET 6.0 veya üzeri Runtime
- Administrator erişimi
- IIS (opsiyonel, ancak önerilir)

## Kurulum

```bash
git clone https://github.com/natiacengiz-eng/aspnet-core-hosting.git
cd aspnet-core-hosting
dotnet restore
dotnet build --configuration Release
```

## Hızlı Başlangıç

### 1. Konfigürasyon Dosyası Oluşturun

```json
{
  "hosting": {
    "baseUrl": "http://localhost:5000",
    "maxApplications": 10,
    "enableSsl": true
  },
  "logging": {
    "level": "Information",
    "directory": "logs"
  },
  "applications": [
    {
      "name": "MyApp1",
      "path": "C:\\Apps\\MyApp1",
      "port": 5001,
      "autoRestart": true,
      "healthCheckUrl": "/health"
    }
  ]
}
```

### 2. Servisi Başlatın

```bash
dotnet CoreHostingService.dll
```

### 3. Yönetim Paneline Erişin

Tarayıcıda `http://localhost:5000` adresine gidin.

## Mimarı

```
aspnet-core-hosting/
├── src/
│   ├── CoreHostingService/           # Ana hosting servisi
│   ├── HostingManager/               # Uygulama yönetimi
│   ├── HealthMonitor/                # Sağlık izleme
│   ├── ApiGateway/                   # REST API
│   └── AdminPanel/                   # Yönetim arayüzü
├── docs/                             # Dokümantasyon
├── samples/                          # Örnek konfigürasyonlar
└── tests/                            # Birim testleri
```

## API Referansı

### Uygulamaları Listele
```
GET /api/applications
```

### Uygulama Başlat
```
POST /api/applications/{id}/start
```

### Uygulama Durdur
```
POST /api/applications/{id}/stop
```

### Uygulama Yeniden Başlat
```
POST /api/applications/{id}/restart
```

### Sağlık Durumu
```
GET /api/health
```

## Lisans

MIT License - Özgürce kullanın ve geliştirin

## Destek

Sorular ve sorunlar için [Issues](https://github.com/natiacengiz-eng/aspnet-core-hosting/issues) sayfasını ziyaret edin.
