# Mikroservis Projesi
Bu proje, mikroservis mimarisi, Onion Mimarisi, CQRS, JWT kimlik doğrulaması, Redis cache, API Gateway (YARP) ve diğer modern teknolojiler kullanılarak geliştirilmiştir. Proje, Auth, Product ve Log mikro servislerini içermekte olup, her biri bağımsız olarak geliştirilmiş ve Docker Compose ile birlikte çalıştırılabilmektedir.


# Gereksinimler

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (manuel çalıştırma için)
- [Docker ve Docker Compose](https://www.docker.com/get-started) (container üzerinden çalıştırmak için)
- [SQL Server](https://www.microsoft.com/en-us/sql-server) (ya da kullanılan veritabanı)
- Diğer bağımlılıklar proje dosyaları içerisindeki NuGet paketleri ile yönetilmektedir.


# Kurulum

## 1. Manuel Çalıştırma

1. **Kodun İndirilmesi/Clone Edilmesi:**  
   Proje deposunu lokal ortamınıza klonlayın:
   ```
   git clone https://github.com/adminyaso/MicroservicesCaseSolution.git
   cd MicroservicesCaseSolution
   ```

2. **Bağımlılıkların Yüklenmesi:**  
   Her bir mikroservis projesinde gerekli NuGet paketlerini yüklemek için:
   ```
   dotnet restore
   ```

3. **Veritabanı Migration İşlemleri:**  
   Projede iki ayrı migration işlemi bulunmaktadır:
   - **Product Servis:**  
     Ürün entity’leri için migration işlemi:
     ```
     cd src/ProductService/ProductService.API
     dotnet ef database update
     ```
   - **Auth Servis:**  
     Kullanıcı ve rol işlemleri için migration:
     ```
     cd src/AuthService/AuthService.API
     dotnet ef database update
     ```
4. **Konfigürasyon İşlemleri:**  
   Her mikroservisi ayrı ayrı konfigüre edilmelidir.:
   ```
   ProductService.API/appsettings.json
   AuthService/AuthService.API/appsettings.json
   LogService/LogService.API/appsettings.json
   ```
    

5. **Uygulamaların Çalıştırılması:**  
   Her mikroservisi ayrı ayrı çalıştırabilirsiniz:
   ```
   dotnet run --project src/ProductService/ProductService.API/ProductService.API.csproj
   dotnet run --project src/AuthService/AuthService.API/AuthService.API.csproj
   dotnet run --project src/LogService/LogService.API/LogService.API.csproj
   dotnet run --project src/ApiGateway/ApiGateway/ApiGateway.csproj
   ```

### 2. Docker Compose ile Çalıştırma

1. **Docker ve Docker Compose’un Yüklü Olduğundan Emin Olun.**
    Manuel Migration:
     cd src/ProductService/ProductService.API
     dotnet ef database update
     ```
     cd src/AuthService/AuthService.API
     dotnet ef database update
    ```
    
2. **Not:**  
   Docker Compose'un içindeki veritabanı bağlantılarını kendi bilgisayarınıza göre kurun. Sql server mode and windows authentication mode'unda olduğundan emin olun. Bilgisayarınızın default ayarlarıyla diğer port ayarları alakalı olabilir!


3. **Docker Compose ile Tüm Servisleri Başlatma:**  
   Proje kök dizininde (docker-compose.yml dosyasının bulunduğu yerde) terminal açın ve aşağıdaki komutu çalıştırın:
   ```
   docker-compose up --build
   ```
   Bu işlem, tüm mikroservisleri (AuthService, ProductService, LogService, API Gateway ve varsa diğer destek servisleri) container içerisinde başlatacaktır.


---

## API Dökümantasyonu

- Swagger(docker) arayüzü üzerinden API dokümantasyonuna erişebilirsiniz:  
  **AuthService:** http://localhost:8088/swagger  
  **ProductService:** http://localhost:8086/swagger  
  **LogService:** http://localhost:8084/swagger  
  Not! : Manuel çalıştırıyorsanız launchSettings.json dosyalarına göz atın.

- Seq arayüzü üzerinden log dokümantasyonuna erişebilirsiniz:  
  **Seq arayüzü:** http://localhost:5341/#/events?range=1d  
---

## Dağıtım

Projeyi dağıtmak için aşağıdaki adımları izleyebilirsiniz:

1. **CI/CD Süreci:**  
   eklenecek...

2. **Docker Image Oluşturma ve Dağıtım:**  
   ```
   docker build -t yourusername/authservice:latest -f src/AuthService/AuthService.API/Dockerfile .
   docker build -t yourusername/productservice:latest -f src/ProductService/ProductService.API/Dockerfile .
   docker build -t yourusername/logservice:latest -f src/LogService/LogService.API/Dockerfile .
   docker build -t yourusername/apigateway:latest -f src/ApiGateway/ApiGateway/Dockerfile .
   ```
   Oluşturduğunuz image’leri Docker Hub veya tercih ettiğiniz container registry’ye push edebilirsiniz.

3. **Dağıtım Platformunda Container’ları Çalıştırma:**  
   Örneğin Kubernetes kullanıyorsanız, ilgili deployment ve service manifest dosyalarını oluşturun ve uygulayın.

---

