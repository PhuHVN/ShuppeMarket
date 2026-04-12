# 🚧 ShuppeMarket - E-Commerce Marketplace Platform

> ⚠️ **DỰ ÁN CÁ NHÂN - ĐANG PHÁT TRIỂN** | This is a personal project under active development

ShuppeMarket là một nền tảng thương mại điện tử hiện đại được xây dựng bằng ASP.NET Core 8.0, cung cấp các tính năng quản lý sản phẩm, tài khoản người dùng, và quản lý người bán.

## 📋 Nội dung

- [Trạng thái phát triển](#trạng-thái-phát-triển)
- [Tổng quan](#tổng-quan)
- [Công nghệ sử dụng](#công-nghệ-sử-dụng)
- [Yêu cầu hệ thống](#yêu-cầu-hệ-thống)
- [Cấu trúc Project](#cấu-trúc-project)
- [Cài đặt & Chạy](#cài-đặt--chạy)
- [Cấu hình](#cấu-hình)
- [API Endpoints](#api-endpoints)
- [Tính năng](#tính-năng)
- [Xác thực](#xác-thực)

## 🚧 Trạng thái phát triển

**Dự án này hiện đang ở giai đoạn phát triển chủ yếu (Early Development Phase)**

### Trạng thái các tính năng:
- ✅ **Hoàn thành**: Authentication (JWT, Google OAuth), Account Management, Product CRUD, Seller Registration
- 🔄 **Đang phát triển**: Shopping Cart, Orders, Payments, Reviews & Ratings
- 📋 **Sắp tới**: Notifications, Analytics Dashboard, Advanced Search Filters, Shipping Integration

### ⚠️ Lưu ý:
- **API có thể thay đổi** mà không có thông báo trước
- **DATABASE SCHEMA** có thể được cập nhật thông qua migrations
- **KHÔNG SỬ DỤNG** cho môi trường production
- Vui lòng **báo cáo bugs** nếu tìm thấy

---

## 🎯 Tổng quan

ShuppeMarket là một nền tảng marketplace toàn diện cho phép:
- **Người dùng** đăng ký tài khoản, đăng nhập, và mua sắm sản phẩm
- **Người bán** đăng ký bán hàng trên nền tảng
- **Quản trị viên** quản lý toàn bộ hệ thống: sản phẩm, tài khoản, người bán, v.v.

## 🛠 Công nghệ sử dụng

| Công nghệ | Phiên bản |
|-----------|---------|
| **ASP.NET Core** | 8.0 |
| **Entity Framework Core** | 8.0.20 |
| **PostgreSQL** | Latest |
| **Redis** | Latest |
| **JWT (JSON Web Token)** | - |
| **Google OAuth 2.0** | - |
| **Cloudinary** | - |
| **FluentValidation** | Latest |
| **AutoMapper** | Latest |
| **Swagger/OpenAPI** | 6.6.2 |

## 📦 Yêu cầu hệ thống

### Phần mềm cần cài đặt
- **.NET 8.0 SDK** trở lên
- **PostgreSQL 12** hoặc cao hơn
- **Redis** (hoặc dùng Upstash Redis)
- **Visual Studio 2022** hoặc **Visual Studio Code** (optional)

### Tài khoản dịch vụ bên thứ ba
- **Google OAuth 2.0** - Để đăng nhập Google
- **Cloudinary** - Để lưu trữ hình ảnh
- **Email SMTP** - Để gửi email xác nhận OTP

## 📁 Cấu trúc Project

Dự án tuân theo kiến trúc **Clean Architecture** với 4 layers chính:

```
ShuppeMarket/
├── ShuppeMarket.API/              # API Layer - Controllers & Middleware
│   ├── Controllers/               # API endpoints
│   │   ├── AuthController.cs      # Xác thực & đăng ký
│   │   ├── AccountController.cs   # Quản lý tài khoản
│   │   ├── ProductController.cs   # Quản lý sản phẩm
│   │   ├── SellerController.cs    # Quản lý người bán
│   │   └── CategoryController.cs  # Quản lý danh mục
│   ├── Middleware/                # Exception handling, logging
│   ├── Program.cs                 # Cấu hình startup
│   └── appsettings.json          # Cấu hình ứng dụng
│
├── ShuppeMarket.Application/       # Business Logic Layer
│   ├── Services/                  # Các dịch vụ ứng dụng
│   │   ├── AuthService.cs         # Logic xác thực
│   │   ├── ProductService.cs      # Logic sản phẩm
│   │   ├── AccountService.cs      # Logic tài khoản
│   │   ├── SellerService.cs       # Logic người bán
│   │   ├── CloudinaryService.cs   # Upload hình ảnh
│   │   ├── EmailService.cs        # Gửi email
│   │   └── ...
│   ├── Interfaces/                # Contracts cho services
│   ├── DTOs/                      # Data Transfer Objects
│   │   ├── MappingProfile.cs      # AutoMapper profiles
│   │   └── ...Dtos/              # DTO cho từng feature
│   └── Validation/                # FluentValidation rules
│
├── ShuppeMarket.Domain/            # Core Domain Layer
│   ├── Entities/                  # Domain entities
│   │   ├── Account.cs
│   │   ├── Product.cs
│   │   ├── Seller.cs
│   │   ├── Category.cs
│   │   └── ...
│   ├── Enums/                     # Enumerations
│   ├── Abstractions/              # Base classes
│   └── ResultError/               # Error handling
│
├── ShuppeMarket.Infrastructure/    # Data Access Layer
│   ├── DatabaseSettings/          # DbContext configuration
│   ├── Migrations/                # EF Core migrations
│   ├── Implementation/            # Repository implementations
│   └── Seed/                      # Database seeding
│
├── ShuppeMarket.sln               # Solution file
├── Dockerfile                     # Docker configuration
└── README.md                      # Tài liệu này
```

## 🚀 Cài đặt & Chạy

### 1. Clone Repository
```bash
git clone <repository-url>
cd ShuppeMarket
```

### 2. Cài đặt Dependencies
```bash
dotnet restore
```

### 3. Cấu hình Connection Strings & API Keys

Chỉnh sửa file `appsettings.Development.json` với thông tin của bạn:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=5432;Database=ShuppeMarketDB;User Id=postgres;Password=your_password;"
  },
  "Jwt": {
    "SecretKey": "your_secret_key_here",
    "Issuer": "ShuppeMarketAPI",
    "Audience": "ShuppeMarketClient",
    "Expiration": 2
  },
  "Authentication": {
    "Google": {
      "ClientId": "your_google_client_id",
      "ClientSecret": "your_google_client_secret"
    }
  },
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "your_email@gmail.com",
    "Password": "your_app_password"
  },
  "Cloudinary": {
    "CloudName": "your_cloud_name",
    "ApiKey": "your_api_key",
    "ApiSecret": "your_api_secret"
  },
  "Redis": {
    "Host": "your_redis_host",
    "Port": 6379,
    "Token": "your_redis_token"
  }
}
```

### 4. Tạo Database & Chạy Migrations
```bash
cd ShuppeMarket.API
dotnet ef database update -p ../ShuppeMarket.Infrastructure/
```

### 5. Chạy ứng dụng
```bash
dotnet run
```

Ứng dụng sẽ khởi động tại `https://localhost:7220` (cổng có thể khác)

### 6. Truy cập Swagger UI
Mở trình duyệt web và truy cập:
```
https://localhost:7220/swagger/index.html
```

## ⚙️ Cấu hình

### Authentication
- **JWT Token**: Dùng để xác thực các request
- **Google OAuth**: Cho phép đăng nhập bằng Google
- **Roles**: Customer, Seller, Admin

### Email Configuration
- Sử dụng SMTP với Gmail
- Gửi OTP xác nhận đăng ký
- Gửi thông báo cho người dùng

### Image Storage
- Cloudinary được sử dụng để lưu trữ hình ảnh sản phẩm
- Hỗ trợ upload từ form data

### Caching
- Redis dùng để cache OTP và các dữ liệu tạm thời
- Cải thiện hiệu suất ứng dụng

## 🔌 API Endpoints

### Authentication (Auth)
| Method | Endpoint | Mô tả |
|--------|----------|--------|
| POST | `/api/v1/auth/register` | Đăng ký tài khoản mới |
| POST | `/api/v1/auth/login` | Đăng nhập bằng email/password |
| POST | `/api/v1/auth/login-google` | Đăng nhập bằng Google |
| POST | `/api/v1/auth/verify-otp` | Xác nhận OTP |
| GET | `/api/v1/auth/me` | Lấy thông tin người dùng hiện tại |

### Accounts
| Method | Endpoint | Mô tả |
|--------|----------|--------|
| POST | `/api/v1/accounts` | Tạo tài khoản mới (Customer) |
| GET | `/api/v1/accounts/{id}` | Lấy thông tin tài khoản |
| GET | `/api/v1/accounts` | Lấy danh sách tất cả tài khoản (Admin) |
| PUT | `/api/v1/accounts` | Cập nhật tài khoản |

### Products
| Method | Endpoint | Mô tả |
|--------|----------|--------|
| POST | `/api/v1/products` | Tạo sản phẩm mới |
| GET | `/api/v1/products/{id}` | Lấy chi tiết sản phẩm |
| GET | `/api/v1/products` | Lấy danh sách sản phẩm (phân trang) |
| DELETE | `/api/v1/products/{id}` | Xóa sản phẩm |

### Sellers
| Method | Endpoint | Mô tả |
|--------|----------|--------|
| POST | `/api/v1/sellers/register/{accountId}` | Đăng ký tài khoản người bán |
| GET | `/api/v1/sellers/{id}` | Lấy thông tin người bán |
| GET | `/api/v1/sellers` | Lấy danh sách người bán (Admin) |
| DELETE | `/api/v1/sellers/{id}` | Xóa người bán (Admin) |

### Categories
| Method | Endpoint | Mô tả |
|--------|----------|--------|
| GET | `/api/v1/categories` | Lấy danh sách danh mục |
| GET | `/api/v1/categories/{id}` | Lấy chi tiết danh mục |
| POST | `/api/v1/categories` | Tạo danh mục mới |
| PUT | `/api/v1/categories/{id}` | Cập nhật danh mục |
| DELETE | `/api/v1/categories/{id}` | Xóa danh mục |

## ✨ Tính năng

### 🔐 Xác thực & Phân quyền
- ✅ Đăng ký tài khoản với xác nhận OTP
- ✅ Đăng nhập bằng email/password
- ✅ Đăng nhập bằng Google OAuth 2.0
- ✅ JWT Token-based authentication
- ✅ Role-based authorization (Customer, Seller, Admin)

### 👥 Quản lý Tài khoản
- ✅ Tạo và cập nhật hồ sơ người dùng
- ✅ Quản lý thông tin cá nhân
- ✅ Phân quyền người dùng

### 📦 Quản lý Sản phẩm
- ✅ CRUD sản phẩm
- ✅ Phân loại sản phẩm theo danh mục
- ✅ Upload hình ảnh sản phẩm (Cloudinary)
- ✅ Phân trang danh sách sản phẩm
- ✅ Tìm kiếm sản phẩm

### 🏪 Quản lý Người bán
- ✅ Đăng ký tài khoản người bán
- ✅ Quản lý shop người bán
- ✅ Phê duyệt/từ chối người bán (Admin)

### 📧 Email & Thông báo
- ✅ Gửi OTP qua email
- ✅ Xác nhận email đăng ký
- ✅ Gửi thông báo cho người dùng

### 🖼️ Upload Hình ảnh
- ✅ Upload hình ảnh sản phẩm
- ✅ Lưu trữ trên Cloudinary
- ✅ Tối ưu hình ảnh tự động

## 🔒 Xác thực

### JWT Token Flow
1. Người dùng đăng nhập với email/password
2. Server tạo JWT token
3. Client gửi token trong header `Authorization: Bearer <token>`
4. Server xác thực token trước khi xử lý request

### Header yêu cầu
```bash
Authorization: Bearer <your_jwt_token>
Content-Type: application/json
```

## 🐳 Docker

Để chạy ứng dụng trong Docker:

```bash
docker build -t shuppemarket:latest .
docker run -p 8080:80 shuppemarket:latest
```

## 📝 Notes & Lưu ý

- � **DỰ ÁN ĐANG PHÁT TRIỂN**: Các tính năng có thể thay đổi bất cứ lúc nào
- 🔑 **Secret Key**: Giữ `SecretKey` (JWT) an toàn, không commit vào git
- 🔑 **API Keys**: Không chia sẻ Cloudinary API key, Google secrets trên công khai
- 🔒 **Database Password**: Sử dụng biến môi trường cho mật khẩu production
- 📊 **Migrations**: Luôn backup database trước khi chạy migrations
- 🧪 **Testing**: Sử dụng appsettings.Development.json cho development
- 🐛 **Bug Reports**: Nếu phát hiện lỗi, vui lòng báo cáo để cải thiện project

## 📧 Liên hệ & Support

- **Email**: noreply@shuppemarket.com
- **Issues**: Báo cáo lỗi qua GitHub Issues

## 📄 License

Dự án này được phát hành dưới [MIT License](LICENSE)

---

**ShuppeMarket** - Build with ❤️ using ASP.NET Core 8.0
