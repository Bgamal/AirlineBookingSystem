# Airline Booking System - Microservices Architecture

A comprehensive microservices-based airline booking system built with .NET 10, demonstrating a distributed, event-driven architecture using RabbitMQ for message bus communication and multiple database technologies.

## Table of Contents

- [Overview](#overview)
- [Project Structure](#project-structure)
- [Microservices Description](#microservices-description)
- [Architecture](#architecture)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Configuration](#configuration)
- [Docker Setup](#docker-setup)
- [Running the Application](#running-the-application)
- [Swagger & API Documentation](#swagger--api-documentation)
- [API Endpoints](#api-endpoints)
- [Database Schemas](#database-schemas)
- [Technologies Used](#technologies-used)
- [Troubleshooting Guide](#troubleshooting-guide)
- [Quick Start Checklist](#quick-start-checklist)

---

## Overview

The Airline Booking System is a distributed microservices application that manages flights, bookings, payments, and notifications. Each microservice is independently deployable and communicates with others through an event-driven architecture using RabbitMQ.

### Key Features

- **Event-Driven Architecture**: Services communicate asynchronously through RabbitMQ
- **Multiple Database Support**: SQL Server, PostgreSQL, and MongoDB
- **Caching**: Redis for distributed caching
- **API Documentation**: Swagger/OpenAPI integration for all services
- **CQRS Pattern**: MediatR for command and query handling
- **Dependency Injection**: Built-in .NET DI container
- **Unit Testing**: Comprehensive test projects for each service

---

## Project Structure

```
AirlineBookingSystem/
├── Services/
│   ├── Flight/                           # Flight Management Service
│   │   ├── AirlineBookingSystem.Flights.Api
│   │   ├── AirlineBookingSystem.Flights.Core
│   │   ├── AirlineBookingSystem.Flights.Application
│   │   └── AirlineBookingSystem.Flights.Infrastructure
│   ├── Booking/                          # Booking Management Service
│   │   ├── AirlineBookingSystem.Bookings.Api
│   │   ├── AirlineBookingSystem.Bookings.Core
│   │   ├── AirlineBookingSystem.Bookings.Application
│   │   └── AirlineBookingSystem.Bookings.Infrastructure
│   ├── Payment/                          # Payment Processing Service
│   │   ├── AirlineBookingSystem.Payments.Api
│   │   ├── AirlineBookingSystem.Payments.Core
│   │   ├── AirlineBookingSystem.Payments.Application
│   │   └── AirlineBookingSystem.Payments.Infrastructure
│   └── Notifications/                    # Notification Service
│       ├── AirlineBookingSystem.Notifications.Api
│       ├── AirlineBookingSystem.Notifications.Core
│       ├── AirlineBookingSystem.Notifications.Application
│       └── AirlineBookingSystem.Notifications.Infrastructure
├── Services-UnitTesting/                 # Unit tests for all services
│   ├── Flight/
│   ├── Booking/
│   ├── Payment/
│   └── Notification/
├── BuildingBlocks/
│   └── AirlineBookingSystem.BuildingBlocks  # Shared libraries and contracts
└── docker-compose.yml                    # Docker orchestration
```

### Project Layer Breakdown

#### Each Microservice Contains 4 Layers:

1. **Api Layer** - REST endpoints and controllers
   - Example: `AirlineBookingSystem.Payments.Api`
   - Contains: Controllers, Program.cs, appsettings.json

2. **Core Layer** - Business entities and interfaces
   - Example: `AirlineBookingSystem.Payments.Core`
   - Contains: Entities, Repositories, Business logic interfaces

3. **Application Layer** - Business logic and handlers
   - Example: `AirlineBookingSystem.Payments.Application`
   - Contains: Handlers (MediatR), Services, DTOs, Commands, Queries

4. **Infrastructure Layer** - Data access and external integrations
   - Example: `AirlineBookingSystem.Payments.Infrastructure`
   - Contains: Repository implementations, Database context, migrations

### Testing Projects

Unit tests are organized by service and layer in `Services-UnitTesting/`:
- `AirlineBookingSystem.Payments.Api.Tests` - API endpoint tests
- `AirlineBookingSystem.Payments.Core.Tests` - Core entity tests
- `AirlineBookingSystem.Payments.Application.Tests` - Handler and business logic tests
- `AirlineBookingSystem.Payments.Infrastructure.Tests` - Repository and data access tests

### Shared Components

**BuildingBlocks** - Common code used across all microservices:
- Event contracts for inter-service communication
- Shared constants and configurations
- Common interfaces and base classes
- Event bus configuration

---

## Microservices Description

### 1. **Flight Service** (Port: 5001)

Manages airline flight information including creation, retrieval, and deletion of flights.

**Database**: SQL Server (with MongoDB support planned)

**Key Components**:
- `FlightsController`: REST endpoints for flight operations
- `IFlightRepository`: Data access layer
- `CreateFlightHandler`, `DeleteFlightHandler`, `GetAllFlightsHandler`: MediatR handlers
- `FlightContext`: Entity Framework context

**Dependencies**:
- MediatR for CQRS pattern
- Dapper for data access
- SQL Server database

**API Endpoints**:
- `GET /api/flights` - Get all flights
- `POST /api/flights` - Create new flight
- `GET /api/flights/{id}` - Get flight by ID
- `DELETE /api/flights/{id}` - Delete flight

---

### 2. **Booking Service** (Port: 5002)

Handles flight bookings and booking management with distributed caching.

**Database**: SQL Server with Redis Cache

**Key Components**:
- `BookingController`: REST endpoints for booking operations
- `IBookingRepository`: Data access layer
- `CreateBookingHandler`, `GetBookingHandler`: MediatR handlers
- `NotificationEventConsumer`: Consumes notification events from RabbitMQ

**Dependencies**:
- MediatR for CQRS pattern
- Redis for distributed caching
- RabbitMQ consumer for event processing
- SQL Server database

**Key Features**:
- Booking creation and retrieval
- Caching of booking data using Redis
- Event subscription to notification events
- Publishes flight booked events

**API Endpoints**:
- `GET /api/bookings` - Get all bookings
- `POST /api/bookings` - Create new booking
- `GET /api/bookings/{id}` - Get booking by ID

---

### 3. **Payment Service** (Port: 5003)

Processes payments and manages payment transactions with PostgreSQL database.

**Database**: PostgreSQL (migrated from SQL Server)

**Key Components**:
- `PaymentsController`: REST endpoints for payment operations
- `IPaymentRepository`: Data access layer
- `ProcessPaymentHandler`, `RefundPaymentHandler`: MediatR handlers
- `FlightBookedConsumer`: Consumes flight booked events from RabbitMQ

**Dependencies**:
- MediatR for CQRS pattern
- Dapper for data access with PostgreSQL
- Npgsql for PostgreSQL connection
- RabbitMQ consumer for event processing

**Key Features**:
- Payment processing
- Payment refunds
- Event-driven payment processing (triggered by flight bookings)
- PostgreSQL database support

**API Endpoints**:
- `GET /api/payments` - Get all payments
- `POST /api/payments` - Process payment
- `GET /api/payments/{id}` - Get payment by ID
- `DELETE /api/payments/{id}` - Refund payment

---

### 4. **Notification Service** (Port: 5004)

Sends notifications based on system events (bookings, payments).

**Database**: SQL Server

**Key Components**:
- `NotificationsController`: REST endpoints for notification operations
- `INotificationRepository`: Data access layer
- `SendNotificationHandler`: MediatR handler
- `PaymentProcessedConsumer`: Consumes payment events from RabbitMQ
- `NotificationService`: Business logic for notification processing

**Dependencies**:
- MediatR for CQRS pattern
- RabbitMQ consumer for event processing
- SQL Server database

**Key Features**:
- Notification creation and sending
- Event-driven notification (triggered by payment processing)
- Support for multiple notification types
- Service layer abstraction for notification logic

**API Endpoints**:
- `GET /api/notifications` - Get all notifications
- `POST /api/notifications` - Send notification
- `GET /api/notifications/{id}` - Get notification by ID

---

### 5. **Building Blocks** (Shared Library)

Contains shared contracts, constants, and utilities used across all microservices.

**Key Contents**:
- `EventBusConstant`: Queue names and event bus constants
- `Contracts`: Event message contracts for inter-service communication
- Common interfaces and base classes

---

## Architecture

### Architectural Patterns

1. **Microservices Pattern**: Each service is independently deployable and focused on a specific business domain
2. **Event-Driven Architecture**: Services communicate asynchronously through RabbitMQ
3. **CQRS (Command Query Responsibility Segregation)**: Separation of read and write operations using MediatR
4. **Repository Pattern**: Data access abstraction with repository interfaces
5. **Dependency Injection**: Built-in .NET service container for loose coupling

### Communication Flow

```
          +--------------------+
          |   Client Request   |
          +--------------------+
                     |
              +---------------+
              |    Gateway    |
              +---------------+
             /        |        \
    +-----------+ +-----------+ +-----------+
    |  Flights  | |  Bookings | |  Payments |
    +-----------+ +-----------+ +-----------+
             \        |        /
              +----------------+
              |    RabbitMQ    |
              |   (Event Bus)  |
              +----------------+
                     |
              +---------------+
              | Notifications |
              +---------------+
```

### Event Flow

1. **Flight Booked Event**: Booking Service publishes → Payment Service consumes → Notification Service publishes follow-up events
2. **Payment Processed Event**: Payment Service publishes → Notification Service consumes
3. **Notification Sent Event**: Notification Service publishes → Booking Service consumes (optional)

---

## Prerequisites

### Software Requirements

- **.NET 10 SDK** ([Download](https://dotnet.microsoft.com/download))
- **Visual Studio 2022** or **Visual Studio Code** with C# extension
- **Git** for version control
- **Docker** and **Docker Compose** (for containerized deployment)

### Required Services

- **RabbitMQ** (Message Bus) - Port: 5672
- **SQL Server** - Port: 1433
- **PostgreSQL** - Port: 5432
- **Redis** - Port: 6379

---

## Installation

### 1. Clone the Repository

```bash
git clone https://github.com/Bgamal/AirlineBookingSystem.git
cd AirlineBookingSystem
```

### 2. Verify .NET Installation

```bash
dotnet --version
# Should show .NET 10.x.x or higher
```

### 3. Restore Dependencies

```bash
dotnet restore
```

### 4. Build the Solution

```bash
dotnet build
```

---

## Configuration

### appsettings.json Configuration

Each microservice has an `appsettings.json` file with service-specific configuration.

#### Payment Service Configuration
**File**: `Services/Payment/AirlineBookingSystem.Payments.Api/appsettings.json`

```json
{
  "EventBusSettings": {
    "HostAddress": "amqp://guest:guest@localhost:5672/"
  },
  "ConnectionStrings": {
    "PostgresConnection": "Host=localhost;Port=5432;Database=PaymentDb;Username=postgres;Password=postgres123"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

#### Booking Service Configuration
**File**: `Services/Booking/AirlineBookingSystem.Bookings.Api/appsettings.json`

```json
{
  "EventBusSettings": {
    "HostAddress": "amqp://guest:guest@localhost:5672/"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BookingDb;Integrated Security=True"
  },
  "CacheSettings": {
    "ConnectionString": "localhost:6379"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

#### Flight Service Configuration
**File**: `Services/Flight/AirlineBookingSystem.Fights.Api/appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=FlightDb;Integrated Security=True"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

#### Notification Service Configuration
**File**: `Services/Notifications/AirlineBookingSystem.Notifications.Api/appsettings.json`

```json
{
  "EventBusSettings": {
    "HostAddress": "amqp://guest:guest@localhost:5672/"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=NotificationDb;Integrated Security=True"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Key Configuration Parameters

| Parameter | Purpose | Example |
|-----------|---------|---------|
| `EventBusSettings:HostAddress` | RabbitMQ connection | `amqp://guest:guest@localhost:5672/` |
| `ConnectionStrings:DefaultConnection` | SQL Server connection | `Data Source=(localdb)\\MSSQLLocalDB;...` |
| `ConnectionStrings:PostgresConnection` | PostgreSQL connection | `Host=localhost;Port=5432;...` |
| `CacheSettings:ConnectionString` | Redis connection | `localhost:6379` |

---

## Docker Setup

### docker-compose.yml

Create a `docker-compose.yml` file in the root directory:

```yaml
version: '3.8'

services:
  # Message Bus - RabbitMQ
  rabbitmq:
    image: rabbitmq:3.13-management
    container_name: airline-rabbitmq
    ports:
      - "5672:5672"
      - "15672:15672"
    environment:
      RABBITMQ_DEFAULT_USER: guest
      RABBITMQ_DEFAULT_PASS: guest
    healthcheck:
      test: rabbitmq-diagnostics ping
      interval: 30s
      timeout: 10s
      retries: 5
    networks:
      - airline-network

  # Primary Database - SQL Server
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: airline-sqlserver
    ports:
      - "1433:1433"
    environment:
      SA_PASSWORD: "YourPassword123!"
      ACCEPT_EULA: "Y"
      MSSQL_PID: "Developer"
    healthcheck:
      test: /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "YourPassword123!" -Q "SELECT 1" || exit 1
      interval: 30s
      timeout: 10s
      retries: 5
    volumes:
      - sqlserver-data:/var/opt/mssql
    networks:
      - airline-network

  # Secondary Database - PostgreSQL
  postgres:
    image: postgres:16-alpine
    container_name: airline-postgres
    ports:
      - "5432:5432"
    environment:
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres123
      POSTGRES_DB: PaymentDb
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 30s
      timeout: 10s
      retries: 5
    volumes:
      - postgres-data:/var/lib/postgresql/data
    networks:
      - airline-network

  # Cache - Redis
  redis:
    image: redis:7-alpine
    container_name: airline-redis
    ports:
      - "6379:6379"
    healthcheck:
      test: ["CMD", "redis-cli", "ping"]
      interval: 30s
      timeout: 10s
      retries: 5
    volumes:
      - redis-data:/data
    networks:
      - airline-network

volumes:
  sqlserver-data:
  postgres-data:
  redis-data:

networks:
  airline-network:
    driver: bridge
```

### Docker Commands

#### Start All Services

```bash
docker-compose up -d
```

**Output**:
```
Creating airline-rabbitmq ... done
Creating airline-sqlserver ... done
Creating airline-postgres ... done
Creating airline-redis ... done
```

#### Check Service Status

```bash
docker-compose ps
```

**Output**:
```
NAME                COMMAND                  SERVICE      STATUS              PORTS
airline-rabbitmq    docker-entrypoint.sh...  rabbitmq     Up 2 minutes        0.0.0.0:5672->5672/tcp, 0.0.0.0:15672->15672/tcp
airline-sqlserver   /opt/mssql/bin/sqlse...  sqlserver    Up 2 minutes        0.0.0.0:1433->1433/tcp
airline-postgres    docker-entrypoint.s...   postgres     Up 2 minutes        0.0.0.0:5432->5432/tcp
airline-redis       redis-server             redis        Up 2 minutes        0.0.0.0:6379->6379/tcp
```

#### View Service Logs

```bash
# View all logs
docker-compose logs -f

# View specific service logs
docker-compose logs -f rabbitmq
docker-compose logs -f sqlserver
docker-compose logs -f postgres
docker-compose logs -f redis
```

#### Access RabbitMQ Management Console

```
URL: http://localhost:15672
Username: guest
Password: guest
```

#### Connect to Databases

**SQL Server**:
```
Server: localhost,1433
Username: sa
Password: YourPassword123!
```

**PostgreSQL**:
```
Host: localhost
Port: 5432
Database: PaymentDb
Username: postgres
Password: postgres123
```

**Redis**:
```
Host: localhost
Port: 6379
```

#### Stop All Services

```bash
docker-compose down
```

#### Stop and Remove Volumes

```bash
docker-compose down -v
```

---

## Running the Application

### 1. Start Infrastructure (Docker)

```bash
docker-compose up -d
```

Wait for all services to be healthy:
```bash
docker-compose ps
```

### 2. Create Databases

**SQL Server** - Create databases using SQL Server Management Studio or sqlcmd:
```sql
CREATE DATABASE FlightDb;
CREATE DATABASE BookingDb;
CREATE DATABASE NotificationDb;
```

**PostgreSQL** - Database is auto-created in docker-compose.yml

### 3. Run Each Microservice

Open separate terminal windows for each service:

#### Flight Service (Dev HTTPS: 63071, HTTP: 63072)
```bash
cd Services/Flight/AirlineBookingSystem.Fights.Api
dotnet run
```

#### Booking Service (Dev HTTPS: 7170, HTTP: 5174)
```bash
cd Services/Booking/AirlineBookingSystem.Bookings.Api
dotnet run
```

#### Payment Service (Dev HTTPS: 58076, HTTP: 58077)
```bash
cd Services/Payment/AirlineBookingSystem.Payments.Api
dotnet run
```

#### Notification Service (Dev HTTPS: 7169, HTTP: 5173)
```bash
cd Services/Notifications/AirlineBookingSystem.Notifications.Api
dotnet run
```

### 4. Verify Services are Running

Check that all services are running:
```bash
# Flight Service
curl http://localhost:63072/swagger/index.html

# Booking Service
curl http://localhost:5174/swagger/index.html

# Payment Service
curl http://localhost:58077/swagger/index.html

# Notification Service
curl http://localhost:5173/swagger/index.html
```

---

## Swagger & API Documentation

All microservices in the Airline Booking System provide interactive API documentation through Swagger UI and OpenAPI specifications. This allows you to explore, test, and interact with the API endpoints directly from your browser.

### Accessing Swagger UI

Once all services are running, access the Swagger UI for each service using the URLs below (matching the launch settings used by `dotnet run`):

| Service | Swagger UI (HTTPS) | Swagger UI (HTTP) | OpenAPI JSON |
|---------|--------------------|-------------------|--------------|
| Flight | [https://localhost:63071/swagger/index.html](https://localhost:63071/swagger/index.html) | [http://localhost:63072/swagger/index.html](http://localhost:63072/swagger/index.html) | `https://localhost:63071/swagger/v1/swagger.json` |
| Booking | [https://localhost:7170/swagger/index.html](https://localhost:7170/swagger/index.html) | [http://localhost:5174/swagger/index.html](http://localhost:5174/swagger/index.html) | `https://localhost:7170/swagger/v1/swagger.json` |
| Payment | [https://localhost:58076/swagger/index.html](https://localhost:58076/swagger/index.html) | [http://localhost:58077/swagger/index.html](http://localhost:58077/swagger/index.html) | `https://localhost:58076/swagger/v1/swagger.json` |
| Notification | [https://localhost:7169/swagger/index.html](https://localhost:7169/swagger/index.html) | [http://localhost:5173/swagger/index.html](http://localhost:5173/swagger/index.html) | `https://localhost:7169/swagger/v1/swagger.json` |

### Using Swagger UI

1. Navigate to any of the Swagger UI URLs above
2. Browse through available endpoints organized by controller
3. Click on any endpoint to expand and view details:
   - Request/Response schemas
   - Required parameters
   - Example values
4. Click **"Try it out"** button to test endpoints
5. Fill in the required parameters or request body
6. Click **"Execute"** to send the request
7. View the response status, headers, and body

### Example: Testing a Flight Creation Endpoint

1. Open [https://localhost:63071/swagger/index.html](https://localhost:63071/swagger/index.html)
2. Find the **POST /api/flights** endpoint
3. Click on it to expand
4. Click **"Try it out"** button
5. Enter flight details in the request body:
   ```json
   {
     "flightNumber": "AA100",
     "departureCity": "New York",
     "arrivalCity": "Los Angeles",
     "departureTime": "2024-01-15T10:00:00Z",
     "arrivalTime": "2024-01-15T13:00:00Z",
     "price": 250.00,
     "availableSeats": 150
   }
   ```
6. Click **"Execute"** to send the request
7. View the response status, headers, and body

### Swagger Configuration

Each microservice is configured with Swagger in its `Program.cs`:

```csharp
// Add Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}
```

### OpenAPI Standards

The API documentation follows the OpenAPI 3.0 specification, which provides:
- **API Discovery**: Automatically generated API documentation
- **Interactive Testing**: Built-in request/response tools
- **Schema Validation**: Automatic request/response schema validation
- **Client Generation**: Tools can generate API client libraries from the OpenAPI spec

---

## API Endpoints

### Flight Service ([HTTP](http://localhost:63072) / [HTTPS](https://localhost:63071))

**Swagger Documentation**: [https://localhost:63071/swagger/index.html](https://localhost:63071/swagger/index.html)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/flights` | Get all flights |
| POST | `/api/flights` | Create a new flight |
| GET | `/api/flights/{id}` | Get flight by ID |
| DELETE | `/api/flights/{id}` | Delete a flight |

**Example Request** (Create Flight):
```bash
curl -X POST http://localhost:63072/api/flights \
  -H "Content-Type: application/json" \
  -d '{
    "flightNumber": "AA100",
    "departureCity": "New York",
    "arrivalCity": "Los Angeles",
    "departureTime": "2024-01-15T10:00:00Z",
    "arrivalTime": "2024-01-15T13:00:00Z",
    "price": 250.00
  }'
```

### Booking Service ([HTTP](http://localhost:5174) / [HTTPS](https://localhost:7170))

**Swagger Documentation**: [https://localhost:7170/swagger/index.html](https://localhost:7170/swagger/index.html)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/bookings` | Get all bookings |
| POST | `/api/bookings` | Create a new booking |
| GET | `/api/bookings/{id}` | Get booking by ID |

**Example Request** (Create Booking):
```bash
curl -X POST http://localhost:5174/api/bookings \
  -H "Content-Type: application/json" \
  -d '{
    "flightId": "f47ac10b-58cc-4372-a567-0e02b2c3d479",
    "passengerName": "John Doe",
    "passengerEmail": "john@example.com",
    "bookingDate": "2024-01-10T00:00:00Z"
  }'
```

### Payment Service ([HTTP](http://localhost:58077) / [HTTPS](https://localhost:58076))

**Swagger Documentation**: [https://localhost:58076/swagger/index.html](https://localhost:58076/swagger/index.html)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/payments` | Get all payments |
| POST | `/api/payments` | Process a payment |
| GET | `/api/payments/{id}` | Get payment by ID |
| DELETE | `/api/payments/{id}` | Refund a payment |

**Example Request** (Process Payment):
```bash
curl -X POST http://localhost:58077/api/payments \
  -H "Content-Type: application/json" \
  -d '{
    "bookingId": "f47ac10b-58cc-4372-a567-0e02b2c3d479",
    "amount": 250.00,
    "paymentDate": "2024-01-10T00:00:00Z"
  }'
```

### Notification Service ([HTTP](http://localhost:5173) / [HTTPS](https://localhost:7169))

**Swagger Documentation**: [https://localhost:7169/swagger/index.html](https://localhost:7169/swagger/index.html)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/notifications` | Get all notifications |
| POST | `/api/notifications` | Send a notification |
| GET | `/api/notifications/{id}` | Get notification by ID |

**Example Request** (Send Notification):
```bash
curl -X POST http://localhost:5173/api/notifications \
  -H "Content-Type: application/json" \
  -d '{
    "recipientEmail": "john@example.com",
    "subject": "Booking Confirmation",
    "body": "Your booking has been confirmed!",
    "notificationType": "email"
  }'
```

---

## Database Schemas

### Flight Service Database (FlightDb - SQL Server)

```sql
CREATE TABLE Flights (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    FlightNumber NVARCHAR(50) NOT NULL,
    DepartureCity NVARCHAR(100) NOT NULL,
    ArrivalCity NVARCHAR(100) NOT NULL,
    DepartureTime DATETIME NOT NULL,
    ArrivalTime DATETIME NOT NULL,
    Price DECIMAL(10, 2) NOT NULL,
    AvailableSeats INT NOT NULL,
    CreatedDate DATETIME DEFAULT GETDATE()
);
```

### Booking Service Database (BookingDb - SQL Server)

```sql
CREATE TABLE Bookings (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    FlightId UNIQUEIDENTIFIER NOT NULL,
    PassengerName NVARCHAR(100) NOT NULL,
    PassengerEmail NVARCHAR(100) NOT NULL,
    BookingDate DATETIME NOT NULL,
    BookingStatus NVARCHAR(50) NOT NULL,
    CreatedDate DATETIME DEFAULT GETDATE()
);
```

### Payment Service Database (PaymentDb - PostgreSQL)

```sql
CREATE TABLE Payments (
    Id UUID PRIMARY KEY,
    BookingId UUID NOT NULL,
    Amount DECIMAL(10, 2) NOT NULL,
    PaymentDate TIMESTAMP NOT NULL,
    PaymentStatus VARCHAR(50) NOT NULL,
    CreatedDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

### Notification Service Database (NotificationDb - SQL Server)

```sql
CREATE TABLE Notifications (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    RecipientEmail NVARCHAR(100) NOT NULL,
    Subject NVARCHAR(200) NOT NULL,
    Body NVARCHAR(MAX) NOT NULL,
    NotificationType NVARCHAR(50) NOT NULL,
    SentDate DATETIME,
    CreatedDate DATETIME DEFAULT GETDATE()
);
```

---

## Technologies Used

### Framework & Runtime
- **.NET 10** - Modern application framework
- **C# 14.0** - Latest language features

### Web & API
- **ASP.NET Core 10** - Web framework
- **Swagger/OpenAPI** - API documentation and testing

### Data Access
- **Dapper** - Lightweight ORM for direct SQL
- **Entity Framework Core** - (Available for use)
- **SQL Server** - Primary relational database
- **PostgreSQL** - Secondary relational database

### Message Bus & Events
- **MassTransit** - Service bus abstraction
- **RabbitMQ** - Message broker for async communication

### CQRS & Patterns
- **MediatR** - Command and Query Handler pattern

### Caching
- **Redis** - Distributed cache
- **StackExchange.Redis** - Redis client library

### Testing
- **xUnit** / **NUnit** - Unit testing frameworks
- **Moq** - Mocking library

### Development Tools
- **Docker** - Containerization
- **Docker Compose** - Multi-container orchestration
- **Git** - Version control

---

## Environment Setup Summary

### Quick Start Checklist

- [ ] Clone repository: `git clone https://github.com/Bgamal/AirlineBookingSystem.git`
- [ ] Install .NET 10 SDK
- [ ] Install Docker and Docker Compose
- [ ] Run `docker-compose up -d` to start infrastructure
- [ ] Create SQL Server databases (FlightDb, BookingDb, NotificationDb)
- [ ] Update `appsettings.json` if needed for your environment
- [ ] Run `dotnet restore` in solution directory
- [ ] Run each microservice with `dotnet run`
- [ ] Access Swagger UI at service URLs
- [ ] Test with provided curl examples

---

## Troubleshooting

### RabbitMQ Connection Issues

If services can't connect to RabbitMQ:

```bash
# Check RabbitMQ is running
docker-compose ps rabbitmq

# View RabbitMQ logs
docker-compose logs rabbitmq

# Restart RabbitMQ
docker-compose restart rabbitmq
```

### Database Connection Issues

```bash
# Check SQL Server is running
docker-compose ps sqlserver

# Test SQL Server connection
sqlcmd -S localhost,1433 -U sa -P "YourPassword123!" -Q "SELECT 1"

# Test PostgreSQL connection
psql -h localhost -U postgres -d PaymentDb -c "SELECT 1"
```

### Redis Connection Issues

```bash
# Test Redis connection
redis-cli ping
# Expected output: PONG
```

### Service Port Issues

If a port is already in use, modify `appsettings.json`:

```json
{
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://localhost:5005"
      }
    }
  }
}
```

---

## Project Statistics

| Metric | Value |
|--------|-------|
| Microservices | 4 |
| API Projects | 4 |
| Core Projects | 4 |
| Application Projects | 4 |
| Infrastructure Projects | 4 |
| Test Projects | 15+ |
| Total Projects | 35+ |
| Target Framework | .NET 10 |
| Language Version | C# 14.0 |

---

## Contributing

1. Create a feature branch: `git checkout -b feature/your-feature`
2. Commit changes: `git commit -am 'Add feature'`
3. Push to branch: `git push origin feature/your-feature`
4. Open a Pull Request

---

## License

This project is licensed under the MIT License - see the LICENSE file for details.

---

## Support & Contact

For issues, questions, or suggestions:
- Create an Issue on GitHub
- Contact the development team

---

## References

- [.NET 10 Documentation](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-10)
- [ASP.NET Core Documentation](https://learn.microsoft.com/en-us/aspnet/core)
- [MassTransit Documentation](https://masstransit.io/)
- [MediatR Documentation](https://github.com/jbogard/MediatR)
- [RabbitMQ Documentation](https://www.rabbitmq.com/documentation.html)
- [Docker Documentation](https://docs.docker.com/)