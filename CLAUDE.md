# CLAUDE.md - Feature Flag System (EbFeatureFlagAspire)

## Project Overview

Sistema de **Feature Flags** multitenant, generico y configurable con diferentes backends de base de datos.
Jerarquia: **Products -> Environments -> Sections -> Feature Keys**.

## Tech Stack

- **Backend**: C# / .NET 9-10, ASP.NET Core Minimal APIs
- **Frontend**: Blazor Interactive Server
- **Orchestration**: .NET Aspire
- **Database**: Cosmos DB (via EF Core)
- **Cache**: Redis (StackExchange.Redis) or In-Memory
- **Architecture**: Layered (Repository -> Provider -> API)

## Solution Structure

```
EB.FeatureFlag.Aspire/
  EB.FeatureFlag.Aspire.AppHost/       # Aspire orchestration
  EB.FeatureFlag.Aspire.ServiceDefaults/ # OpenTelemetry, health checks, resilience
  EB.FeatureFlag.Aspire.ApiService/    # API backend (minimal APIs) - needs endpoints
  EB.FeatureFlag.Aspire.Web/           # Blazor frontend - needs feature flag UI
EB.FeatureFlag.Data/                   # Aggregation project
  EB.FeatureFlag.Data.IRepository/     # Repository interfaces + DTOs
  EB.FeatureFlag.Data.Repository.CosmosDb/ # Cosmos DB implementation
  EB.FeatureFlag.Data.IProvider/       # Provider interface
  EB.FeatureFlag.Data.Provider/        # Provider implementation (cache-aware)
  EB.FeatureFlag.Data.ICache/          # Cache interface
  EB.FeatureFlag.Data.Cache.InMemory/  # In-memory cache
  EB.FeatureFlag.Data.Cache.Redis/     # Redis cache
```

## Coding Standards

### C# Conventions
- Use **primary constructors** (modern C# style)
- **Enums** must have explicit numeric values assigned (e.g., `Boolean = 0, LargeString = 1`)
- Enums must be stored as `int` in the database
- Use C# 12+ features and patterns
- Follow SOLID principles strictly
- Strong typing everywhere - no `dynamic` or `object` unless absolutely required
- Prefer Minimal APIs over Controllers for new endpoints
- Use `record` types for DTOs when appropriate

### Architecture Rules
- Repository pattern with interface abstraction (`IRepository` -> Implementation)
- Provider layer handles business logic and cache management
- Cache is optional (null-safe pattern already in place)
- Cosmos DB partition key strategy: all entities partition by `ProductId`
- 10-minute default cache expiration

### General Rules
- No placeholder code - everything must be functional
- No code generation in bulk - work in stages, verify each one
- Modular and extensible design - easy to add new Feature Key types
- Build and verify before moving to next stage

## Build & Run

```bash
# Build the solution
dotnet build EB.FeatureFlag.Aspire/EB.FeatureFlag.Aspire.sln

# Run with Aspire (from AppHost)
dotnet run --project EB.FeatureFlag.Aspire/EB.FeatureFlag.Aspire.AppHost
```

## Configuration

Settings in `ApiService/appsettings.json`:
- `FeatureFlag_RepositoryType`: "Cosmos"
- `FeatureFlag_RepositoryConnectionString`: connection string
- `FeatureFlag_CacheType`: "Redis" or "InMemory"
- `FeatureFlag_CacheConnectionString`: Redis connection string

## Current State (as of initial setup)

### Completed
- Data models and entities (Product, Environment, Section, FeatureKey)
- Repository interfaces and Cosmos DB implementations (CRUD)
- Cache abstraction with In-Memory and Redis implementations
- Provider layer with automatic cache management
- Aspire orchestration (AppHost)
- Blazor frontend scaffolding
- FeatureKeyType enum: Boolean=0, LargeString=1, StringCollection=2, JsonCollection=3

### Not Yet Implemented
- API endpoints for feature flag management (ApiService only has weather placeholder)
- DI wiring in ApiService Program.cs for repositories/providers/cache
- Access Key generation and rotation logic
- Feature Key type validation (boolean, string, collection, JSON)
- Regex validation for String Collections
- External source fetching
- Public consumption endpoint (SDK/client)
- Blazor UI for feature flag management
- Unit tests

## Development Phases

The project follows a 6-stage plan:
1. Data Modeling (mostly done) - entities, DTOs, repositories
2. Product/Environment CRUD + Access Key rotation
3. Feature Key management + type validators
4. Advanced validations (regex for String Collections)
5. External sources (dynamic fetching from URLs/APIs)
6. Public consumption API (protected by Access Keys, with caching)
