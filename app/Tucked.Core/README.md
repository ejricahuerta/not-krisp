# Tucked.Core - Backend API

This is the backend API for Tucked, built with .NET 9 and PostgreSQL.

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/install/)

## Development Setup

1. Clone the repository
2. Navigate to the Tucked.Core directory
3. Create a `.env` file with the following variables:
   ```env
   JWT_KEY=your-256-bit-secret
   JWT_ISSUER=https://your-domain
   JWT_AUDIENCE=https://your-domain
   ```
4. Run the development environment:
   ```bash
   docker-compose up -d
   ```
5. The API will be available at `http://localhost:8080`
6. Swagger UI is available at `http://localhost:8080/swagger`
7. Seq log viewer is available at `http://localhost:5341`

## Configuration

The application uses the following configuration sources in order of precedence:
1. Environment variables
2. `.env` file
3. `appsettings.{Environment}.json`
4. `appsettings.json`

### Available Settings

- `ConnectionStrings:DefaultConnection`: PostgreSQL connection string
- `Jwt:Key`: JWT signing key (min 256 bits)
- `Jwt:Issuer`: JWT issuer URL
- `Jwt:Audience`: JWT audience URL

## Docker Deployment

The application is containerized and can be deployed using Docker Compose:

```bash
# Build and start containers
docker-compose up -d --build

# View logs
docker-compose logs -f

# Stop containers
docker-compose down

# Stop containers and remove volumes
docker-compose down -v
```

### Container Configuration

The application runs in two containers:
1. API (`api`):
   - .NET 9 Web API
   - Exposed on port 8080
   - Health check endpoint at `/health`
   - Structured logging with Serilog
2. Database (`db`):
   - PostgreSQL 16
   - Exposed on port 5432
   - Persistent volume for data storage

### Health Checks

The API includes health checks for:
- Application status
- Database connectivity
- External service dependencies

Access the health check endpoint at `http://localhost:8080/health`

### Logging

The application uses Serilog for structured logging with the following sinks:
- Console (with ANSI colors)
- File (daily rolling, in `/app/logs`)
- Seq (available at `http://localhost:5341`)

Log files are stored in the `logs` directory and include:
- Application logs
- Request/response logs
- Error details
- Performance metrics

## API Documentation

The API is documented using Swagger/OpenAPI. When running in development mode, you can access:
- Swagger UI: `http://localhost:8080/swagger`
- OpenAPI JSON: `http://localhost:8080/swagger/v1/swagger.json`

## Database Migrations

Migrations are automatically applied on startup. To manually manage migrations:

```bash
# Create a new migration
dotnet ef migrations add MigrationName

# Apply migrations
dotnet ef database update

# Remove last migration
dotnet ef migrations remove
```

## Security

- All endpoints (except health check and swagger) require JWT authentication
- Tokens are validated for:
  - Valid signature
  - Expiration
  - Issuer
  - Audience
- Database passwords and JWT keys should be provided via environment variables
- HTTPS is enforced in production
- Structured logging excludes sensitive data

## Troubleshooting

1. Database Connection Issues:
   - Check PostgreSQL container is running: `docker ps`
   - Verify connection string in environment variables
   - Check database logs: `docker-compose logs db`

2. API Issues:
   - Check API logs: `docker-compose logs api`
   - Verify health check endpoint
   - Check Seq logs for detailed error information

3. Container Issues:
   - Rebuild containers: `docker-compose up -d --build`
   - Check Docker logs: `docker-compose logs`
   - Verify environment variables in `.env` file 