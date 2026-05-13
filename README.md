# Smart Wallet SA

Smart Wallet SA is a full-stack fintech wallet management system built with ASP.NET Core Web API and Angular.

The application allows users to securely register, authenticate, manage wallets, deposit funds, withdraw money, transfer money between users, and view transaction history through a modern web interface.

---

## Features

### Authentication & Security

- User registration
- User login
- JWT authentication
- Protected API endpoints
- Angular route guards
- HTTP interceptor for JWT token handling

### Wallet Management

- View wallet balance
- Deposit money
- Withdraw money
- Transfer money between users
- Automatic wallet creation during registration

### Transactions

- Transaction history
- Transaction date formatting
- Real-time balance updates

### Frontend

- Angular standalone components
- Responsive dashboard UI
- Navigation system
- Protected frontend routes
- Service-based architecture

---

## Tech Stack

### Backend

- ASP.NET Core Web API
- C#
- Entity Framework Core
- SQL Server LocalDB
- JWT Authentication

### Frontend

- Angular
- TypeScript
- HTML
- CSS

---

## Architecture

### Backend Structure

- Controllers
- Services
- Entity Framework DbContext
- Models
- JWT Authentication
- RESTful API Architecture

### Frontend Structure

- Pages
- Services
- Models
- Guards
- Interceptors

---

## API Endpoints

### Authentication

- `POST /api/auth/register`
- `POST /api/auth/login`

### Wallet

- `GET /api/wallet`
- `POST /api/wallet/deposit`
- `POST /api/wallet/withdraw`
- `POST /api/wallet/transfer`

### Transactions

- `GET /api/transactions`

---

## Getting Started

### Backend Setup

1. Navigate to the backend project:

```bash
cd SmartWalletSA
```

2. Restore packages:

```bash
dotnet restore
```

3. Apply migrations:

```bash
dotnet ef database update
```

4. Run the API:

```bash
dotnet run
```

---

### Frontend Setup

1. Navigate to Angular project:

```bash
cd smart-wallet-sa-client
```

2. Install dependencies:

```bash
npm install
```

3. Run Angular application:

```bash
ng serve
```

4. Open:

```text
http://localhost:4200
```

---

## Future Improvements

- Financial analytics charts
- Email notifications
- Mobile responsive improvements
- Transaction filtering
- Admin dashboard
- Docker deployment
- Cloud deployment
- Unit testing

---

## Author

Pilato Mmatshipyane
