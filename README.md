# 🎬 Showmax Clone — Full-Stack Streaming Platform

A fully functional video-on-demand streaming platform built entirely in **C#** using **Blazor WebAssembly** for the frontend and **ASP.NET Core Web API** for the backend.

---

## 📸 Features

- 🔐 **Authentication** — Register, login, JWT tokens, role-based access (Admin/User)
- 🏠 **Homepage** — Featured hero banner, Movies & Series rows
- 🎬 **Content Browsing** — Browse by genre, language, type
- 🔍 **Search** — Real-time search by title and genre
- ▶️ **Video Player** — Stream videos with progress saving and resume
- 📋 **Watchlist** — Add/remove content to personal watchlist
- 💳 **Subscriptions** — Basic (R99), Standard (R149), Premium (R199) via Stripe
- 👤 **User Profile** — Edit profile, change password
- 🛠️ **Admin Panel** — Add/edit/delete content, upload via Cloudinary, view stats
- 🌐 **Landing Page** — Marketing page for non-logged in users

---

## 🏗️ Tech Stack

| Layer | Technology |
|-------|-----------|
| Frontend | Blazor WebAssembly (.NET 8) |
| Backend | ASP.NET Core Web API (.NET 8) |
| ORM | Entity Framework Core |
| Database | PostgreSQL |
| Cache | Redis |
| Auth | ASP.NET Core Identity + JWT |
| Media | Cloudinary .NET SDK |
| Payments | Stripe .NET SDK |
| Email | SendGrid .NET SDK |
| Deployment | Microsoft Azure |

---

## 📁 Project Structure

```
Showmax/
├── Showmax.Client/          # Blazor WebAssembly frontend
│   ├── Layout/              # NavBar, MainLayout
│   ├── Pages/               # All Razor pages
│   │   ├── Home.razor
│   │   ├── Landing.razor
│   │   ├── Movies.razor
│   │   ├── Series.razor
│   │   ├── Search.razor
│   │   ├── ContentDetail.razor
│   │   ├── Watch.razor
│   │   ├── MyWatchlist.razor
│   │   ├── Plans.razor
│   │   ├── UserProfile.razor
│   │   ├── Login.razor
│   │   ├── Register.razor
│   │   └── Admin.razor
│   ├── Services/            # ApiService.cs
│   └── wwwroot/             # CSS, index.html
│
├── Showmax.Server/          # ASP.NET Core Web API backend
│   ├── Controllers/         # All API controllers
│   │   ├── AuthController.cs
│   │   ├── ContentController.cs
│   │   ├── WatchlistController.cs
│   │   ├── PaymentsController.cs
│   │   ├── AdminController.cs
│   │   ├── ProfileController.cs
│   │   └── UploadController.cs
│   ├── Data/                # AppDbContext.cs
│   ├── Services/            # CloudinaryService.cs
│   └── Migrations/          # EF Core migrations
│
└── Showmax.Shared/          # Shared models & DTOs
    ├── Models/              # Entity classes
    └── DTOs/                # Data Transfer Objects
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or VS Code
- A [Cloudinary](https://cloudinary.com/) account (free)
- A [Stripe](https://stripe.com/) account (free test mode)

---

### 1. Clone the Repository

```bash
git clone https://github.com/YOUR_USERNAME/showmax-clone.git
cd showmax-clone
```

---

### 2. Configure `appsettings.json`

Open `Showmax.Server/appsettings.json` and fill in your values:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=ShowmaxDb;Username=postgres;Password=YOUR_PASSWORD"
  },
  "JwtSettings": {
    "Key": "ShowmaxSuperSecretKey1234567890AbCdEfGh",
    "Issuer": "ShowmaxServer",
    "Audience": "ShowmaxClient",
    "ExpiryDays": 7
  },
  "Stripe": {
    "SecretKey": "sk_test_YOUR_STRIPE_KEY",
    "PublishableKey": "pk_test_YOUR_STRIPE_KEY"
  },
  "Cloudinary": {
    "CloudName": "YOUR_CLOUD_NAME",
    "ApiKey": "YOUR_API_KEY",
    "ApiSecret": "YOUR_API_SECRET"
  }
}
```

---

### 3. Configure Client API URL

Open `Showmax.Client/Program.cs` and set your server URL:

```csharp
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5053/")
});
```

---

### 4. Run Database Migrations

Open **Package Manager Console** in Visual Studio, set default project to `Showmax.Server` and run:

```powershell
Add-Migration InitialCreate
Update-Database
```

---

### 5. Run the Application

**Option A — Visual Studio:**
1. Right-click Solution → Set Startup Projects → Multiple Startup Projects
2. Set both `Showmax.Server` and `Showmax.Client` to **Start**
3. Press **F5**

**Option B — Command Line:**

Terminal 1 (Server):
```bash
cd Showmax.Server
dotnet run
```

Terminal 2 (Client):
```bash
cd Showmax.Client
dotnet run
```

---

### 6. Default Admin Account

Once the server starts, an admin account is automatically created:

| Field | Value |
|-------|-------|
| Email | `admin@showmax.com` |
| Password | `Admin@1234` |

---

## 🔑 API Endpoints

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/auth/register` | Register new user | Public |
| POST | `/api/auth/login` | Login and get JWT | Public |
| GET | `/api/content` | Get all content | Public |
| GET | `/api/content/{id}` | Get content by ID | Public |
| GET | `/api/content/search?query=` | Search content | Public |
| GET | `/api/content/genre/{genre}` | Filter by genre | Public |
| GET | `/api/watchlist` | Get user watchlist | 🔒 User |
| POST | `/api/watchlist/{contentId}` | Add to watchlist | 🔒 User |
| DELETE | `/api/watchlist/{contentId}` | Remove from watchlist | 🔒 User |
| GET | `/api/payments/plans` | Get subscription plans | Public |
| POST | `/api/payments/create-checkout-session` | Start Stripe checkout | 🔒 User |
| GET | `/api/profile` | Get user profile | 🔒 User |
| PUT | `/api/profile` | Update profile | 🔒 User |
| GET | `/api/admin/content` | Admin: get all content | 🔒 Admin |
| POST | `/api/admin/content` | Admin: add content | 🔒 Admin |
| PUT | `/api/admin/content/{id}` | Admin: update content | 🔒 Admin |
| DELETE | `/api/admin/content/{id}` | Admin: delete content | 🔒 Admin |
| GET | `/api/admin/stats` | Admin: dashboard stats | 🔒 Admin |

---

## 🗃️ Database Schema

| Table | Description |
|-------|-------------|
| Users | ASP.NET Identity users with FullName and AvatarUrl |
| Profiles | Sub-profiles per user (Kids, Main etc.) |
| Content | Movies and series metadata |
| Episodes | Individual episodes for series |
| Watchlists | User saved content |
| WatchHistory | Playback progress tracking |
| Subscriptions | Stripe subscription records |

---

## 🌐 Deployment

The application is designed to deploy on **Microsoft Azure**:

- **Showmax.Client** → Azure Static Web Apps
- **Showmax.Server** → Azure App Service
- **Database** → Azure Database for PostgreSQL

---

## 📱 Pages & Routes

| Route | Page | Auth Required |
|-------|------|---------------|
| `/landing` | Landing / Marketing Page | No |
| `/` | Homepage | Yes |
| `/movies` | Movies listing | Yes |
| `/series` | Series listing | Yes |
| `/search` | Search page | Yes |
| `/content/{id}` | Content detail | No |
| `/watch/{id}` | Video player | Yes |
| `/watchlist` | My watchlist | Yes |
| `/plans` | Subscription plans | No |
| `/profile` | User profile | Yes |
| `/login` | Login | No |
| `/register` | Register | No |
| `/admin` | Admin panel | Yes (Admin) |

---

## 🧑‍💻 Built With

- [Blazor WebAssembly](https://learn.microsoft.com/en-us/aspnet/core/blazor/) — C# frontend framework
- [ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/) — Backend API framework
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/) — ORM
- [PostgreSQL](https://www.postgresql.org/) — Relational database
- [Cloudinary](https://cloudinary.com/) — Media storage and streaming
- [Stripe](https://stripe.com/) — Payment processing
- [SendGrid](https://sendgrid.com/) — Email service

---

## 📄 License

This project is built for academic purposes as a university assignment.

---

## 👤 Author

**Phumlile Mtshali**
University Project — 2026
