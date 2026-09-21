# Vacay — Georgia travel booking platform

Vacay is a full-stack ASP.NET Core booking website for hotels and car rentals in Tbilisi, Batumi, and Kutaisi. It is designed as a course project that demonstrates a real booking flow, a REST API, an SQL database, email notifications, Docker, and a Linux-server deployment path.

## What the website does

- Searches and displays hotel rooms with city, dates, guest count, prices, and taxes.
- Lets travellers create accounts, log in, reset or change passwords, update their profile, save favourites, make and cancel hotel reservations, and view their trips.
- Includes car rental in all three cities, driving-licence validation, rental confirmation, and cancellation flows.
- Sends account, reservation, rental, password-reset, and cancellation emails through Brevo.
- Provides profile reservations, saved hotels, car rentals, dark mode, and English, Georgian, and Russian user interfaces.
- Includes city-guide articles and a host-message experience.

## Architecture

```text
Browser UI (HTML, CSS, JavaScript)
        ↓ HTTP / JSON
ASP.NET Core controllers (REST API + Swagger)
        ↓
Application service layer (AuthService, ReservationService,
CarRentalService, HostChatService)
        ↓
Repository / Entity Framework Core infrastructure
        ↓
SQL database (PostgreSQL in Docker, SQL Server LocalDB for direct local development)
```

The application uses JWT authentication. The browser stores the signed-in session token locally, so a traveller stays signed in until they log out. The service layer contains the booking rules and email workflow; controllers do not directly contain those rules.

## Run locally with Docker (recommended demonstration)

Docker Desktop starts **two Linux containers**:

1. `web` — the Vacay ASP.NET Core website and API.
2. `database` — PostgreSQL 16, which stores the actual website data.

```powershell
docker compose up --build
```

Then open:

- Website: <http://localhost:8080>
- Swagger API: <http://localhost:8080/swagger>

The PostgreSQL database is persisted in the Docker named volume `vacay_postgres_data`. Stopping the containers does not remove reservations, users, favourites, or rentals. To stop the visible log window, use `Ctrl+C`; to run it in the background use `docker compose up -d --build`.

### Email configuration for Docker

Copy `.env.example` to `.env` and set the real Brevo API key and verified sender email. `.env` is ignored by Git and Docker build context, so credentials are never published.

```powershell
Copy-Item .env.example .env
docker compose up --build
```

## Direct local development with Visual Studio

When launched using Visual Studio or `dotnet run`, the project uses SQL Server LocalDB through the `SqlServer` connection string in `appsettings.json`. When launched with Docker, it uses the PostgreSQL connection string injected by Docker Compose. Therefore the site is connected to an SQL database in both modes, but they are intentionally separate development databases.

Use `dotnet user-secrets` for Brevo values during direct development; do not put a real API key or password in source control.

## Deploy to a Linux server / DigitalOcean

The repository includes `docker-compose.production.yml`, `Caddyfile`, and `.env.production.example` for a real Linux deployment. A DigitalOcean Ubuntu Droplet is a Linux virtual server, which satisfies the usual "run it in Docker and put it on a server" project requirement.

On the server:

```bash
git clone git@github.com:Eleneavalishvili/vacay-hotel.git
cd vacay-hotel
cp .env.production.example .env
nano .env
docker compose -f docker-compose.production.yml up -d --build
```

Set unique values in `.env` for `VACAY_DB_PASSWORD` and `JWT_KEY`, then add the real Brevo API key and verified sender email. After Docker finishes, the public website is available at:

```text
http://YOUR_SERVER_IP
http://YOUR_SERVER_IP/swagger
```

The production setup runs PostgreSQL, the ASP.NET application, and Caddy as separate containers. Caddy exposes only port 80 publicly and forwards traffic to the application. A domain is optional for the teacher demonstration; adding one later allows HTTPS to be enabled with a small Caddy configuration change.

## Security notes

- `.env`, local data, build output, and local SSH credentials are ignored by Git.
- Docker ignores `.env` and local SSH credentials, so a production image cannot accidentally contain them.
- Only `.env.example` files with placeholders are safe to commit.
- Never share a Brevo API key, password, token, or the private SSH key.

## Project links

- Repository: <https://github.com/Eleneavalishvili/vacay-hotel>
- Local Docker website: <http://localhost:8080>
- Local Docker Swagger: <http://localhost:8080/swagger>
