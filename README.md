# not-krisp

A meeting assistant that captures audio, transcribes conversations, and creates actionable tickets in project management tools.

## Tech Stack

- **Frontend**: SvelteKit + ShadCN UI
- **Backend**: .NET 9 Web API
- **Database**: PostgreSQL
- **Containerization**: Docker
- **Hosting**: Render

## Project Structure

```
not-krisp/
├── src/
│   ├── frontend/     # SvelteKit frontend application
│   ├── backend/      # .NET 9 Web API backend
│   └── docker/       # Docker configuration files
└── docs/            # Project documentation
```

## Environment Variables

The following environment variables are required to run the application. Create a `.env` file in the root directory and add these variables:

```env
# ASP.NET Core Environment
ASPNETCORE_ENVIRONMENT=Development

# GitHub OAuth Settings
GITHUB_CLIENT_ID=your_github_client_id_here
GITHUB_CLIENT_SECRET=your_github_client_secret_here

# JWT Settings
JWT_SECRET_KEY=your_jwt_secret_key_here_min_32_chars

# PostgreSQL Settings
POSTGRES_USER=notkrisp_user
POSTGRES_PASSWORD=strong_password_here
POSTGRES_DB=notkrisp_db
```

## Getting Started

### Prerequisites

- Node.js 18+
- .NET 9 SDK
- Docker and Docker Compose
- PostgreSQL

### Local Development

1. Clone the repository
2. Set up environment variables
3. Run `docker-compose up` to start all services

## License

MIT 