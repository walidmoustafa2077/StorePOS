# StorePOS - Point of Sale System

A modern, RESTful Point of Sale (POS) system built with .NET 9 and minimal APIs. This system provides comprehensive retail management capabilities including product management, sales processing, user management, and inventory tracking.

## 🚀 Features

### Core Functionality
- **Product Management**: Create, update, delete, and search products with category support
- **Sales Processing**: Complete sales transactions with multiple payment methods
- **User Management**: Role-based user system (Admin, Manager, Cashier)
- **Inventory Tracking**: Real-time stock management and updates
- **Authentication & Authorization**: JWT-based security with role-based access control
- **Shopping Cart**: Session-based cart management for multi-item sales

### Technical Features
- **RESTful API**: Clean, documented endpoints following REST principles
- **Swagger Documentation**: Interactive API documentation at `/swagger`
- **Entity Framework Core**: Database abstraction with In-Memory database for development
- **Repository Pattern**: Clean architecture with separation of concerns
- **Service Layer**: Business logic encapsulation
- **JWT Authentication**: Secure token-based authentication
- **Integration Guide**: Complete developer documentation with code examples

## 🛠️ Technology Stack

- **.NET 9.0**: Latest .NET framework
- **ASP.NET Core**: Web API framework with minimal APIs
- **Entity Framework Core**: ORM for data access
- **JWT Bearer Authentication**: Secure authentication
- **Swagger/OpenAPI**: API documentation
- **In-Memory Database**: For development and testing

## 📋 Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Visual Studio 2022 or VS Code
- Git

## 🚀 Getting Started

### 1. Clone the Repository
```bash
git clone <repository-url>
cd StorePOS
```

### 2. Restore Dependencies
```bash
dotnet restore
```

### 3. Build the Solution
```bash
dotnet build
```

### 4. Run the Application
```bash
cd src/StorePOS.Api
dotnet run
```

The API will be available at:
- HTTP: `http://localhost:5062`
- HTTPS: `https://localhost:7099`
- Swagger UI: `https://localhost:7099/swagger`

## 📁 Project Structure

```
StorePOS/
├── src/
│   ├── StorePOS.Api/                 # Web API project
│   │   ├── Endpoints/                # API endpoint definitions
│   │   │   ├── AuthEndpoints.cs      # Authentication endpoints
│   │   │   ├── ProductEndpoints.cs   # Product management
│   │   │   ├── SaleEndpoints.cs      # Sales processing
│   │   │   ├── UserEndpoints.cs      # User management
│   │   │   └── DOCs/                 # Endpoint documentation
│   │   ├── Authorization/            # Security & authorization
│   │   └── Properties/               # Launch settings
│   │
│   ├── StorePOS.Domain/              # Business logic & data
│   │   ├── Models/                   # Entity models
│   │   ├── Services/                 # Business services
│   │   ├── Data/                     # Data access layer
│   │   │   └── Repositories/         # Repository implementations
│   │   ├── DTOs/                     # Data transfer objects
│   │   ├── Enums/                    # Enumerations
│   │   ├── Extensions/               # Extension methods
│   │   └── Helpers/                  # Utility helpers
│   │
│   └── StorePOS.Api.Integration.Guide/ # Developer documentation
│       ├── index.html                # Main integration guide
│       ├── sections/                 # Documentation sections
│       ├── examples/                 # Code examples
│       │   ├── csharp/              # C# client examples
│       │   ├── javascript/          # JavaScript examples
│       │   ├── python/              # Python examples
│       │   └── powershell/          # PowerShell examples
│       └── assets/                   # CSS and JS assets
│
├── StorePOS.sln                     # Solution file
├── .gitignore                       # Git ignore rules
└── README.md                        # This file
```

## 🔐 Authentication

The system uses JWT (JSON Web Tokens) for authentication. To access protected endpoints:

1. **Login** using the `/auth/login` endpoint with valid credentials
2. **Receive JWT token** in the response
3. **Include token** in subsequent requests: `Authorization: Bearer <token>`

### Default Users
The system initializes with default users:
- **Admin**: Full system access
- **Manager**: Product and user management
- **Cashier**: Sales processing only

## 📚 API Documentation

### Core Endpoints

#### Authentication
- `POST /auth/login` - User login
- `POST /auth/refresh` - Refresh JWT token

#### Products
- `GET /products` - List all products
- `GET /products/{id}` - Get product by ID
- `POST /products` - Create new product
- `PUT /products/{id}` - Update product
- `DELETE /products/{id}` - Delete product
- `PUT /products/{id}/stock` - Update stock

#### Sales
- `GET /sales` - List sales
- `POST /sales` - Create new sale
- `GET /sales/{id}` - Get sale details

#### Users
- `GET /users` - List users
- `POST /users` - Create user
- `PUT /users/{id}` - Update user
- `DELETE /users/{id}` - Delete user

### Interactive Documentation
Visit `/swagger` when running the application for complete interactive API documentation.

## 🔧 Configuration

### Application Settings
Configure the application through `appsettings.json`:

```json
{
  "JwtSettings": {
    "SecretKey": "your-secret-key-here",
    "Issuer": "StorePOS",
    "Audience": "StorePOS-Users",
    "ExpirationInMinutes": 60
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=storepos.db"
  }
}
```

### Environment Variables
- `ASPNETCORE_ENVIRONMENT`: Set to `Development`, `Staging`, or `Production`
- `JWT_SECRET`: Override JWT secret key
- `CONNECTION_STRING`: Override database connection

## 🧪 Development

### Running Tests
```bash
dotnet test
```

### Code Style
The project follows standard .NET coding conventions and includes:
- Nullable reference types enabled
- Implicit usings enabled
- Modern C# language features

### Adding New Features
1. Create models in `StorePOS.Domain/Models/`
2. Add DTOs in `StorePOS.Domain/DTOs/`
3. Implement services in `StorePOS.Domain/Services/`
4. Create endpoints in `StorePOS.Api/Endpoints/`
5. Add documentation in `StorePOS.Api/Endpoints/DOCs/`

## 🚀 Deployment

### Docker (Future)
```dockerfile
# Dockerfile will be added in future versions
```

### Production Considerations
- Replace In-Memory database with SQL Server/PostgreSQL
- Configure proper JWT secrets
- Set up HTTPS certificates
- Configure logging and monitoring
- Implement rate limiting
- Add input validation and sanitization

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📞 Support

For support and questions:
- Create an issue in the repository
- Check the integration guide at `/src/StorePOS.Api.Integration.Guide/`
- Review the API documentation at `/swagger`

## 🗺️ Roadmap

- [ ] Database migrations and seeding
- [ ] Unit and integration tests
- [ ] Docker containerization
- [ ] Real-time notifications
- [ ] Report generation
- [ ] Barcode scanning support
- [ ] Receipt printing
- [ ] Customer management
- [ ] Loyalty program
- [ ] Multi-store support

---

**Built with ❤️ using .NET 9 and modern development practices**
