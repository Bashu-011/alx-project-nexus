# E-Commerce Backend API

A production-ready e-commerce backend API built with ASP.NET Core 8.0, featuring JWT authentication, shopping cart functionality, and M-Pesa STK Push payment integration.

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=flat&logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![Docker](https://img.shields.io/badge/Docker-2496ED?style=flat&logo=docker&logoColor=white)](https://www.docker.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)


---

## Table of Contents

- [Features](#-features)
- [Technologies Used](#-technologies-used)
- [Architecture](#-architecture)
- [Getting Started](#-getting-started)
  - [Prerequisites](#prerequisites)
  - [Local Development Setup](#local-development-setup)
  - [Docker Deployment](#docker-deployment)
- [API Documentation](#-api-documentation)
  - [Authentication](#authentication)
  - [Products](#products)
  - [Categories](#categories)
  - [Shopping Cart](#shopping-cart)
  - [Orders & Checkout](#orders--checkout)
- [M-Pesa Integration](#-m-pesa-integration)
- [Environment Variables](#-environment-variables)
- [Database Schema](#-database-schema)
- [Security](#-security)
- [Contributing](#-contributing)
- [License](#-license)

---

## Features

### Core Functionality
-  **User Authentication & Authorization** - JWT-based secure authentication
- **Product Management** - CRUD operations with filtering, sorting, and pagination
-  **Category Management** - Organize products into categories
- **Shopping Cart** - Add, update, remove items with real-time stock validation
-  **Order Management** - Complete order tracking and history
-  **M-Pesa Payment Integration** - STK Push for seamless mobile payments

### Technical Features
- **RESTful API Design** - Clean, intuitive endpoints
-  **Swagger/OpenAPI Documentation** - Interactive API documentation
- **Input Validation** - FluentValidation for robust data validation
- **Clean Architecture** - Separation of concerns with layered architecture
-  **Entity Framework Core** - Code-first database migrations
-  **AutoMapper** - Efficient object-to-object mapping
-  **Docker Support** - Containerized deployment
-  **Error Handling** - Comprehensive error responses
-  **Logging** - Structured logging with Serilog

---

##  Technologies Used

### Backend Framework
- **ASP.NET Core 8.0** - High-performance, cross-platform framework
- **C# 12** - Modern, type-safe programming language

### Database
- **PostgreSQL 16** - Robust, open-source relational database
- **Entity Framework Core 8.0** - Object-Relational Mapper (ORM)

### Authentication & Security
- **JWT (JSON Web Tokens)** - Stateless authentication
- **BCrypt.Net** - Password hashing
- **HTTPS/TLS** - Encrypted communication

### Payment Integration
- **M-Pesa Daraja API** - Mobile money payment processing
- **STK Push** - Customer-initiated payment prompts

### Libraries & Tools
- **AutoMapper** - Object mapping
- **FluentValidation** - Input validation
- **Swashbuckle (Swagger)** - API documentation
- **Npgsql** - PostgreSQL .NET driver

### Development & Deployment
- **Docker** - Containerization
- **Railway** - Cloud hosting platform
- **Git** - Version control

---

## Architecture

The project follows **Clean Architecture** principles with clear separation of concerns:

```
E-Commerce Backend

E_Commerce.API (Presentation Layer)
Controllers/          # API endpoints
Program.cs           # Application entry point
Dockerfile           # Container configuration

E_Commerce.Application (Application Layer)
DTOs/                # Data Transfer Objects
Interfaces/          # Service contracts
Services/            # Business logic
Mappings/            # AutoMapper profiles
Validators/          # Input validation

E_Commerce.Core (Domain Layer)
Entities/            # Domain models
Interfaces/          # Repository contracts

E_Commerce.Infrastructure (Infrastructure Layer)
Data/                # Database context
Repositories/        # Data access logic
```

### Design Patterns Used
- **Repository Pattern** - Data access abstraction
- **Dependency Injection** - Loose coupling
- **DTO Pattern** - API separation
- **Service Layer Pattern** - Business logic encapsulation

---

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL 16+](https://www.postgresql.org/download/)
- [Git](https://git-scm.com/downloads)
- [Docker](https://www.docker.com/get-started) (optional, for containerized deployment)

### Local Development Setup

#### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/ecommerce-backend.git
cd ecommerce-backend
```

#### 2. Configure Database

```bash
# Create PostgreSQL database
createdb -U postgres ecommerce_db
```

#### 3. Configure Secrets

**Option A: User Secrets (Recommended)**

```bash
cd E_Commerce.API
dotnet user-secrets init

# Database
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=ecommerce_db;Username=postgres;Password=YOUR_PASSWORD"

# JWT
dotnet user-secrets set "JwtSettings:SecretKey" "your-secret-key-minimum-32-characters-long"

# M-Pesa (Sandbox)
dotnet user-secrets set "MpesaSettings:ConsumerKey" "your-consumer-key"
dotnet user-secrets set "MpesaSettings:ConsumerSecret" "your-consumer-secret"
dotnet user-secrets set "MpesaSettings:PassKey" "your-passkey"
```

**Option B: .env File**

```bash
cp E_Commerce.API/.env.example E_Commerce.API/.env
# Edit .env and add your credentials
```

#### 4. Apply Database Migrations

```bash
cd E_Commerce.API
dotnet ef database update --project ../E_Commerce.Infrastructure/E_Commerce.Infrastructure.csproj
```

#### 5. Run the Application

```bash
dotnet run
```

**Navigate to:** `https://localhost:5001`

---

### Docker Deployment

#### Build and Run Locally

```bash
# Build image
docker build -t ecommerce-api .

# Run container
docker run -p 8080:8080 \
  -e DATABASE_URL="postgresql://user:pass@host:5432/db" \
  -e JWT_SECRET="your-secret-key" \
  -e MPESA_CONSUMER_KEY="your-key" \
  -e MPESA_CONSUMER_SECRET="your-secret" \
  ecommerce-api
```

#### Deploy to Railway

```bash
# Push to GitHub
git push origin main

# Railway auto-deploys from GitHub
# Configure environment variables in Railway dashboard
```

---

## API Documentation

### Base URL
```
Production: https://alx-project-nexus-production-cbfd.up.railway.app/swagger/index.html
Local: https://localhost:5001
```

### Authentication

All endpoints except `/auth/register` and `/auth/login` require a JWT token in the Authorization header on SWAGGER, remember to add "Bearer" before the token:

```
Authorization: Bearer <your-jwt-token>
```

---

### Authentication

#### Register User

```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePass123!",
  "confirmPassword": "SecurePass123!",
  "firstName": "John",
  "lastName": "Doe"
}
```

**Response:**
```json
{
  "success": true,
  "message": "User registered successfully",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "expiresAt": "2025-01-27T10:30:00Z"
  }
}
```

#### Login

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePass123!"
}
```

**Response:** Same as registration

---

### Products

#### Get All Products (with pagination & filtering)

```http
GET /api/products?pageNumber=1&pageSize=10&categoryId=<guid>&minPrice=100&maxPrice=1000&searchTerm=phone&sortBy=price&isDescending=false
```

**Query Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `pageNumber` | integer | Page number (default: 1) |
| `pageSize` | integer | Items per page (default: 10) |
| `categoryId` | guid | Filter by category |
| `minPrice` | decimal | Minimum price filter |
| `maxPrice` | decimal | Maximum price filter |
| `searchTerm` | string | Search in name/description |
| `sortBy` | string | Sort field (price, name, createdAt) |
| `isDescending` | boolean | Sort direction |

**Response:**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": "guid",
        "name": "iPhone 15",
        "description": "Latest Apple smartphone",
        "price": 999.99,
        "stockQuantity": 50,
        "categoryName": "Electronics",
        "imageUrl": "https://image"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 5,
    "totalCount": 50,
    "hasPrevious": false,
    "hasNext": true
  }
}
```

#### Get Product by ID

```http
GET /api/products/{id}
```

#### Create Product (Admin only)

```http
POST /api/products
Authorization: Bearer <admin-token>
Content-Type: application/json

{
  "name": "iPhone 15 Pro",
  "description": "Latest Apple flagship phone",
  "price": 999.99,
  "stockQuantity": 100,
  "imageUrl": "https://url",
  "categoryId": "category-guid"
}
```

#### Update Product (Admin only)

```http
PUT /api/products/{id}
Authorization: Bearer <admin-token>
Content-Type: application/json

{
  "name": "iPhone 15 Pro Max",
  "description": "Updated description",
  "price": 1099.99,
  "stockQuantity": 75,
  "imageUrl": "https://...",
  "categoryId": "guid",
  "isActive": true
}
```

#### Delete Product (Admin only)

```http
DELETE /api/products/{id}
Authorization: Bearer <admin-token>
```

---

### Categories

#### Get All Categories

```http
GET /api/categories
```

#### Get Category by ID

```http
GET /api/categories/{id}
```

#### Create Category (Admin only)

```http
POST /api/categories
Authorization: Bearer <admin-token>
Content-Type: application/json

{
  "name": "Electronics",
  "description": "Electronic devices and gadgets"
}
```

#### Update Category (Admin only)

```http
PUT /api/categories/{id}
Authorization: Bearer <admin-token>
Content-Type: application/json

{
  "name": "Consumer Electronics",
  "description": "Updated description"
}
```

#### Delete Category (Admin only)

```http
DELETE /api/categories/{id}
Authorization: Bearer <admin-token>
```

---

### Shopping Cart

#### Get Cart

```http
GET /api/cart
Authorization: Bearer <token>
```

**Response:**
```json
{
  "success": true,
  "data": {
    "id": "cart-guid",
    "items": [
      {
        "id": "item-guid",
        "productId": "product-guid",
        "productName": "iPhone 15",
        "productImageUrl": "https://...",
        "unitPrice": 999.99,
        "quantity": 2,
        "totalPrice": 1999.98
      }
    ],
    "totalItems": 2,
    "subTotal": 1999.98
  }
}
```

#### Add to Cart

```http
POST /api/cart/items
Authorization: Bearer <token>
Content-Type: application/json

{
  "productId": "product-guid",
  "quantity": 2
}
```

#### Update Cart Item

```http
PUT /api/cart/items/{cartItemId}
Authorization: Bearer <token>
Content-Type: application/json

{
  "quantity": 5
}
```

#### Remove from Cart

```http
DELETE /api/cart/items/{cartItemId}
Authorization: Bearer <token>
```

#### Clear Cart

```http
DELETE /api/cart
Authorization: Bearer <token>
```

---

### Orders & Checkout

#### Checkout (Initiate M-Pesa Payment)

```http
POST /api/orders/checkout
Authorization: Bearer <token>
Content-Type: application/json

{
  "phoneNumber": "254712345678"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Payment initiated successfully",
  "data": {
    "orderId": "order-guid",
    "orderNumber": "ORD-20250126-12345",
    "totalAmount": 1999.98,
    "message": "Please check your phone and enter M-Pesa PIN",
    "checkoutRequestId": "ws_CO_..."
  }
}
```

**What happens next:**
1.  STK push sent to customer's phone
2. Customer enters M-Pesa PIN
3.  M-Pesa processes payment
4.  Callback updates order status
5.  Stock automatically reduced

#### Get Order History

```http
GET /api/orders
Authorization: Bearer <token>
```

#### Get Order by ID

```http
GET /api/orders/{orderId}
Authorization: Bearer <token>
```

**Response:**
```json
{
  "success": true,
  "data": {
    "id": "order-guid",
    "orderNumber": "ORD-20250126-12345",
    "totalAmount": 1999.98,
    "status": "Completed",
    "phoneNumber": "254712345678",
    "mpesaReceiptNumber": "RGF123XYZ",
    "paymentDate": "2025-01-26T14:30:00Z",
    "items": [
      {
        "productName": "iPhone 15",
        "quantity": 2,
        "unitPrice": 999.99,
        "totalPrice": 1999.98
      }
    ],
    "createdAt": "2025-01-26T14:25:00Z"
  }
}
```

---

## M-Pesa Integration

### How It Works

1. **User adds items to cart** - Products saved temporarily
2. **User clicks checkout** - Order created with status "Pending"
3. **API calls M-Pesa STK Push** - Push notification sent to phone
4. **User enters M-Pesa PIN** - Payment processed by Safaricom
5. **M-Pesa sends callback** - API receives payment confirmation
6. **Order status updated** - "Completed" + stock reduced
7. **User receives confirmation** - Receipt number saved

### Sandbox Testing

**Test Phone Numbers:**
- `2547xxxxxxxx` (Safaricom sandbox)

**Test Scenarios:**
| Phone Number | Result |
|--------------|--------|
| `2547xxxxxxxx` | Success |
| `2547xxxxxxxx` | Insufficient Balance |
| `2547xxxxxxxx` | Wrong PIN |

### Callback Endpoint

M-Pesa calls this endpoint after payment:

```http
POST /api/mpesa/callback
Content-Type: application/json

{
  "Body": {
    "stkCallback": {
      "MerchantRequestID": "...",
      "CheckoutRequestID": "...",
      "ResultCode": 0,
      "ResultDesc": "Success",
      "CallbackMetadata": {
        "Item": [
          { "Name": "Amount", "Value": 100 },
          { "Name": "MpesaReceiptNumber", "Value": "RGF123XYZ" }
        ]
      }
    }
  }
}
```

**This endpoint is publicly accessible (no authentication) as M-Pesa servers call it.

---

## Environment Variables

### Required Variables

```bash
# Database
DATABASE_URL=postgresql://user:password@host:5432/database

# JWT Authentication
JWT_SECRET=your-secret-key-minimum-32-characters-long
JWT_ISSUER=E-CommerceAPI
JWT_AUDIENCE=ECommerceClient
JWT_EXPIRY_MINUTES=60

# M-Pesa Configuration
MPESA_CONSUMER_KEY=your_consumer_key
MPESA_CONSUMER_SECRET=your_consumer_secret
MPESA_PASSKEY=your_passkey
MPESA_SHORTCODE=174379
MPESA_ENVIRONMENT=sandbox
MPESA_CALLBACK_URL=https://your-domain.com/api/mpesa/callback

# ASP.NET Core
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080
```

### How to Set (Railway)

1. Go to Railway dashboard
2. Select your project
3. Click "Variables" tab
4. Add each variable
5. Redeploy

---

## Database Schema

### Entity Relationships

```
User (1) --- (Many) Cart
                        -
                        --- (Many) CartItem ???? (1) Product
                        
User (1) --- (Many) Order
                        -
                        --- (Many) OrderItem ???? (1) Product

Category (1) --- (Many) Product
```

### Core Tables

- **Users** - Authentication and user profiles
- **Categories** - Product categorization
- **Products** - Product catalog with pricing and stock
- **Carts** - Active shopping carts
- **CartItems** - Products in cart
- **Orders** - Completed/pending orders with M-Pesa tracking
- **OrderItems** - Products in order (historical snapshot)

---

## Security

### Implemented Security Measures

**Password Hashing** - BCrypt with salt  
**JWT Authentication** - Stateless, secure tokens  
**HTTPS Only** - All traffic encrypted  
**Input Validation** - FluentValidation on all inputs  
**SQL Injection Prevention** - Parameterized queries (EF Core)  
**CORS Configuration** - Controlled cross-origin access  
**Role-Based Authorization** - Admin vs Customer permissions  
**Secrets Management** - Environment variables, no hardcoded secrets  

### Password Requirements

- Minimum 8 characters
- At least 1 uppercase letter
- At least 1 lowercase letter
- At least 1 number
- At least 1 special character

---

## Testing

### Manual Testing with Swagger

1. Navigate to `https://alx-project-nexus-production-cbfd.up.railway.app/swagger/index.html`
2. Click **"Authorize"** button
3. Enter JWT token: `Bearer <token>`
4. Test endpoints interactively

### Testing with cURL

```bash
# Register
curl -X POST https://your-app.railway.app/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"test@test.com","password":"Test123!","confirmPassword":"Test123!","firstName":"Test","lastName":"User"}'

# Login
curl -X POST https://your-app.railway.app/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"test@test.com","password":"Test123!"}'

# Get Products
curl -X GET https://your-app.railway.app/api/products \
  -H "Authorization: Bearer <token>"
```

---

## API Response Format

All API responses follow this consistent structure:

### Success Response

```json
{
  "success": true,
  "message": "Operation successful",
  "data": { },
  "errors": []
}
```

### Error Response

```json
{
  "success": false,
  "message": "Error message",
  "data": null,
  "errors": [
    "Detailed error 1",
    "Detailed error 2"
  ]
}
```

### HTTP Status Codes

| Code | Meaning |
|------|---------|
| `200` | Success |
| `201` | Created |
| `400` | Bad Request (validation failed) |
| `401` | Unauthorized (missing/invalid token) |
| `403` | Forbidden (insufficient permissions) |
| `404` | Not Found |
| `500` | Internal Server Error |

---

## Future of the Project

### Phase 1 (Current) 
- [x] User authentication
- [x] Product management
- [x] Shopping cart
- [x] M-Pesa payments
- [x] Order tracking

### Phase 2 (Planned)
- [ ] Email notifications
- [ ] Order cancellation
- [ ] Product reviews & ratings
- [ ] Wishlist functionality
- [ ] Admin dashboard
- [ ] Analytics & reporting

### Phase 3 (Future)
- [ ] Multiple payment methods
- [ ] Shipping integration
- [ ] Inventory alerts
- [ ] Promotional codes/coupons
- [ ] Multi-currency support


---

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.








