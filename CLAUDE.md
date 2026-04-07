# CLAUDE.md - Feature Flag System

## Summary
Multitenant feature flag system:
- Products → Environments → Sections → Feature Keys
- Backend: C# / .NET 9-10, Minimal APIs
- Frontend: Blazor Server
- DB: Cosmos DB; Cache: Redis or InMemory
- Orchestration: .NET Aspire

## Key Principles
- Layered architecture: Repository → Provider → API
- Cosmos partition key: `ProductId`
- Enums explicit as `int`
- C# 12+ patterns, strong typing, no placeholders
- Minimal APIs, DTO request models, `record` types

## Configuration
`ApiService/appsettings.json`:
- `FeatureFlag_RepositoryType`
- `FeatureFlag_RepositoryConnectionString`
- `FeatureFlag_CacheType`
- `FeatureFlag_CacheConnectionString`

## Status
- Phases 1-6 completed: models, repos, cache, provider, key rotation, validators, regex, external sources, public SDK API
- Pending: Blazor management UI, tests

## Main Endpoints
- `GET/POST/PUT/DELETE /api/products`, `POST /api/products/{id}/rotate-keys`
- `GET/POST/PUT/DELETE /api/products/{productId}/environments`, `POST /api/environments/{id}/rotate-keys`
- `GET/POST/PUT/DELETE /api/environments/{environmentId}/sections`
- `GET/POST/PUT/DELETE /api/sections/{sectionId}/feature-keys`, `GET/PUT/DELETE /api/feature-keys/{id}`

## SDK Endpoint (Phase 6)
- `GET /api/sdk/feature-flags` — Headers: `X-Product-Key`, `X-Environment-Key`
- Validates access keys (primary or secondary), returns all sections + keys with resolved external values
- Response cached 5 min via `FeatureFlag:Sdk:{productId}:{environmentId}`

## FeatureKey Validation
- `IFeatureKeyValueValidator` per type, `LargeString`/`StringCollection` use `ValidationRegex`

## Build
```bash
dotnet build EB.FeatureFlag.Aspire/EB.FeatureFlag.Aspire.sln
dotnet run --project EB.FeatureFlag.Aspire/EB.FeatureFlag.Aspire.AppHost
```