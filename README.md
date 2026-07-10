# Full-Stack JWT Authentication Solution

ASP.NET Core 8 Web API + React (Vite) with JWT authentication, refresh tokens, role-based authorization, and SQL Server.

## Project Structure

```
AuthSolution/
├── API/                    # ASP.NET Core 8 Web API
│   ├── Controllers/
│   ├── Services/
│   ├── Repositories/
│   ├── Models/
│   ├── DTOs/
│   ├── Data/
│   ├── Helpers/
│   ├── Middleware/
│   └── Interfaces/
├── Frontend/               # React + Vite
│   └── src/
├── deploy/                 # VPS docker-compose (pull images)
│   ├── docker-compose.yml
│   ├── .env.example
│   └── deploy.sh
├── .github/workflows/      # GitHub Actions CI/CD
│   └── docker-publish.yml
└── Database/
    └── InitialCreate.sql   # EF Core migration SQL script
```

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/) and npm
- SQL Server or SQL Server LocalDB

## Sample Test User

| Field    | Value              |
|----------|--------------------|
| Username | `admin`            |
| Email    | `admin@example.com`|
| Password | `Admin@123`        |
| Role     | `Admin`            |

## Backend Setup

### 1. Configure connection string

Edit `API/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AuthDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

For SQL Server Express, use:

```
Server=localhost\\SQLEXPRESS;Database=AuthDb;Trusted_Connection=True;TrustServerCertificate=True;
```

### 2. Apply database migrations

**Option A – EF Core CLI (recommended):**

```bash
cd API
dotnet ef database update
```

**Option B – SQL script:**

Run `Database/InitialCreate.sql` against your SQL Server database.

### 3. Run the API

```bash
cd API
dotnet run --launch-profile https
```

- API: `https://localhost:7001`
- Swagger: `https://localhost:7001/swagger`

## Frontend Setup

### 1. Install dependencies

```bash
cd Frontend
npm install
```

### 2. Configure API URL

Copy `.env.example` to `.env`:

```bash
cp .env.example .env
```

Default API URL: `https://localhost:7001/api`

### 3. Run the React app

```bash
npm run dev
```

Open `http://localhost:5173` and log in with the sample admin user.

## API Endpoints

| Method | Endpoint            | Auth     | Description              |
|--------|---------------------|----------|--------------------------|
| POST   | `/api/auth/register`| Public   | Register new user        |
| POST   | `/api/auth/login`   | Public   | Login, returns JWT       |
| POST   | `/api/auth/refresh` | Public   | Refresh access token     |
| GET    | `/api/auth/profile` | Required | Current user profile     |
| GET    | `/api/users`        | Required | List all users           |
| GET    | `/api/users/admin-only` | Admin | Admin-only example   |

## Authentication Flow

```
React Login Form
       │
       ▼
POST /api/auth/login
       │
       ▼
ASP.NET Core API validates credentials (BCrypt)
       │
       ▼
Generate JWT + Refresh Token
       │
       ▼
React stores tokens in localStorage
       │
       ▼
Axios interceptor attaches Bearer token
       │
       ▼
Protected routes & API calls (Dashboard, /users)
```

## Key Features

### Backend
- JWT authentication with configurable expiration
- BCrypt password hashing
- Refresh token rotation
- Role-based authorization (`Admin`, `User`)
- Repository pattern + service layer
- AutoMapper for DTO mapping
- Global exception handling middleware
- Swagger with JWT Bearer support
- CORS configured for React dev server

### Frontend
- React Router protected routes
- Context API for auth state
- Axios interceptors for token attach & refresh
- Bootstrap UI with validation and loading states

## JWT Configuration

Located in `API/appsettings.json`:

```json
"JwtSettings": {
  "Secret": "YourSuperSecretKeyForJWT_MustBeAtLeast32Characters!",
  "Issuer": "AuthAPI",
  "Audience": "AuthAPIClient",
  "AccessTokenExpirationMinutes": 60,
  "RefreshTokenExpirationDays": 7
}
```

> **Production:** Replace the JWT secret with a strong key stored in environment variables or Azure Key Vault.

## Testing with Swagger

1. Run the API and open Swagger UI.
2. Call `POST /api/auth/login` with:
   ```json
   {
     "usernameOrEmail": "admin",
     "password": "Admin@123"
   }
   ```
3. Copy the `accessToken` from the response.
4. Click **Authorize**, enter `Bearer <your-token>`, and test protected endpoints.

## Troubleshooting

| Issue | Solution |
|-------|----------|
| CORS errors | Ensure API is running and `Cors:AllowedOrigins` includes `http://localhost:5173` |
| SSL certificate errors | Run `dotnet dev-certs https --trust` |
| Database connection failed | Verify SQL Server/LocalDB is running and connection string is correct |
| 401 on protected routes | Check token expiry; frontend will attempt refresh automatically |

---

## Docker (Local Build & Test)

Build and run all services locally from source:

```bash
docker compose -f docker-compose.dev.yml up --build
```

| Service  | URL |
|----------|-----|
| Frontend | http://localhost:8080 |
| API      | http://localhost:5000 |
| Swagger  | http://localhost:8080/swagger |

Login: `admin` / `Admin@123`

---

## CI/CD — GitHub Actions

On every push to `main`/`master`, GitHub Actions automatically:

1. Builds Docker images for API and Frontend
2. Pushes them to **GitHub Container Registry (GHCR)**

### Workflow file

`.github/workflows/docker-publish.yml`

### Published images

```
ghcr.io/<your-github-username>/auth-api:latest
ghcr.io/<your-github-username>/auth-frontend:latest
```

### Setup steps

1. **Push code to GitHub:**
   ```bash
   git init
   git add .
   git commit -m "Initial commit with Docker and CI/CD"
   git remote add origin https://github.com/YOUR_USERNAME/AuthSolution.git
   git push -u origin main
   ```

2. **Enable GitHub Actions** — runs automatically on push.

3. **Make GHCR packages public** (so VPS can pull without auth):
   - Go to GitHub → Your Profile → Packages
   - Open `auth-api` and `auth-frontend`
   - Package settings → Change visibility → **Public**

   Or keep private and authenticate on VPS:
   ```bash
   docker login ghcr.io -u YOUR_GITHUB_USERNAME -p YOUR_GITHUB_PAT
   ```

---

## VPS Deployment (Pull Images)

After GitHub Actions builds the images, deploy on your VPS:

### 1. Install Docker on VPS

```bash
curl -fsSL https://get.docker.com | sh
sudo usermod -aG docker $USER
```

### 2. Copy deploy folder to VPS

```bash
scp -r deploy/ user@your-vps-ip:/opt/authsolution/
```

### 3. Configure environment

```bash
cd /opt/authsolution
cp .env.example .env
nano .env
```

Set these values in `.env`:

```env
REGISTRY=ghcr.io/your-github-username
IMAGE_TAG=latest
FRONTEND_URL=http://YOUR_VPS_IP
HTTP_PORT=80

# SQL Server already running on VPS host (localhost:1433)
DB_HOST=host.docker.internal
DB_PORT=1433
DB_NAME=SalmanDB
DB_USER=sa
DB_PASSWORD=YourStrong@Passw0rd

JWT_SECRET=YourSuperSecretKeyForJWT_MustBeAtLeast32Characters!
```

> **Note:** `DB_HOST=host.docker.internal` lets the API container reach SQL Server on the VPS host. Do not use `localhost` inside `.env` on the VPS — that would point to the container itself, not the host.

> **Migrations:** EF Core runs `Database.Migrate()` automatically when the API container starts. Tables are created/updated in `SalmanDB` on first run.

### 4. Pull images and start

**Important:** Use `docker-compose.yml` (production). Do **NOT** use `docker-compose.dev.yml` on VPS.

```bash
cp .env.example .env
nano .env

docker login ghcr.io   # only if packages are private
docker compose pull
docker compose up -d
```

Or use the deploy script:

```bash
chmod +x deploy.sh
./deploy.sh
```

### 5. Verify

Open `http://YOUR_VPS_IP` in your browser and log in with `admin` / `Admin@123`.

### Update deployment (after new push to main)

```bash
cd /opt/authsolution
docker compose pull
docker compose up -d
```

### Architecture on VPS

```
Internet
    │
    ▼
┌─────────────────┐
│  frontend :80   │  nginx → serves React + proxies /api
└────────┬────────┘
         │ /api
         ▼
┌─────────────────┐
│   api :8080     │  ASP.NET Core + auto EF migrations
└────────┬────────┘
         │ host.docker.internal:1433
         ▼
┌─────────────────┐
│  SQL Server     │  SalmanDB on VPS host (localhost:1433)
└─────────────────┘
```

### Connect to VPS SQL from your PC (SSH tunnel)

If SQL Server listens only on the VPS localhost, tunnel port **14330** on your machine to **1433** on the VPS:

```bash
ssh -L 14330:localhost:1433 user@your-vps-ip
```

Then connect from SSMS, Azure Data Studio, or local dev:

```
Server=localhost,14330;Database=SalmanDB;User Id=sa;Password=YourPassword;TrustServerCertificate=True;
```

For local API dev against the tunneled database, set in `API/appsettings.Development.json`:

```json
"DefaultConnection": "Server=localhost,14330;Database=SalmanDB;User Id=sa;Password=YourPassword;TrustServerCertificate=True;Encrypt=False;"
```

## License

MIT
