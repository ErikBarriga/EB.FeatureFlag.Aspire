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
  EB.FeatureFlag.Aspire.ApiService/    # API backend (minimal APIs)
    Endpoints/                         # Minimal API endpoint mappings
    Models/                            # API request/response models
  EB.FeatureFlag.Aspire.Web/           # Blazor frontend - needs feature flag UI
EB.FeatureFlag.Data/                   # Aggregation project
  EB.FeatureFlag.Data.IRepository/     # Repository interfaces + DTOs
  EB.FeatureFlag.Data.Repository.CosmosDb/ # Cosmos DB implementation
  EB.FeatureFlag.Data.IProvider/       # Provider interface + AccessKeyGenerator
    Validation/                        # IFeatureKeyValueValidator, FeatureKeyValidationException
  EB.FeatureFlag.Data.Provider/        # Provider implementation (cache-aware)
    Validators/                        # FeatureKey type validators (Boolean, LargeString, etc.)
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

## Current State

### Completed (Phase 1 - Data Modeling)
- Data models and entities (Product, Environment, Section, FeatureKey)
- Repository interfaces and Cosmos DB implementations (CRUD)
- Cache abstraction with In-Memory and Redis implementations
- Provider layer with automatic cache management
- Aspire orchestration (AppHost)
- Blazor frontend scaffolding
- FeatureKeyType enum: Boolean=0, LargeString=1, StringCollection=2, JsonCollection=3
- DI aggregation project (`EB.FeatureFlag.Data`) with `AddFeatureFlagData()` extension

### Completed (Phase 2 - Product/Environment CRUD + Access Keys)
- DI wiring in ApiService `Program.cs` via `AddFeatureFlagData()`
- `AccessKeyGenerator` (cryptographically secure, Base64-encoded 32 bytes)
- Auto-generation of Primary/Secondary Access Keys on Product/Environment creation
- Key rotation logic: Primary → Secondary, new Primary generated
- Product API endpoints: GET/POST/PUT/DELETE `/api/products` + POST `/api/products/{id}/rotate-keys`
- Environment API endpoints: GET/POST/PUT/DELETE under `/api/products/{productId}/environments` and `/api/environments/{id}` + POST `/api/environments/{id}/rotate-keys`
- Request models (`CreateProductRequest`, `UpdateProductRequest`, `CreateEnvironmentRequest`, `UpdateEnvironmentRequest`) to avoid exposing DTOs with Access Keys in write operations
- Removed weather placeholder endpoint

### Completed (Phase 3 - Section/FeatureKey CRUD + Type Validators)
- Fixed partition key propagation: `ISectionRepository.AddAsync(dto, productId)` and `IFeatureKeyRepository.AddAsync(dto, environmentId, productId)`
- Provider resolves parent IDs (Environment→ProductId, Section→EnvironmentId→ProductId) before creating entities
- Feature Key value validation via Strategy pattern:
  - `IFeatureKeyValueValidator` interface + `IFeatureKeyValueValidatorFactory` for dispatch
  - `BooleanValueValidator`: only `true`/`false` (bool, string, or JsonElement)
  - `LargeStringValueValidator`: must be a non-null string
  - `StringCollectionValueValidator`: must be an array of strings
  - `JsonCollectionValueValidator`: must be an array of valid JSON objects/strings
  - `FeatureKeyValidationException` thrown on invalid values → API returns 400 BadRequest
- Validators handle both native types and `JsonElement` (from API JSON deserialization)
- Section API endpoints: CRUD under `/api/environments/{environmentId}/sections` and `/api/sections/{id}`
- FeatureKey API endpoints: CRUD under `/api/sections/{sectionId}/feature-keys` and `/api/feature-keys/{id}`
- Validators registered in DI via `AddFeatureFlagValidators()` in ServiceCollectionExtensions

### Completed (Phase 4 - Regex Validation for StringCollection)
- Added `ValidationRegex` optional field to `FeatureKeyDto`, `FeatureKeyEntity`, mapper, repository, and API request models
- `StringCollectionValueValidator` validates each item against the regex pattern when provided
- Invalid regex patterns throw `FeatureKeyValidationException` with clear message
- Regex compiled with `RegexOptions.Compiled` and 5-second timeout for safety
- Error messages include: item index, item value, and the pattern it failed against
- `LargeStringValueValidator` also validates the string value against the regex when provided (e.g. email, URL format)
- Other validators (Boolean, JsonCollection) accept but ignore the regex parameter

### Not Yet Implemented
- External source fetching
- Public consumption endpoint (SDK/client, protected by Access Keys)
- Blazor UI for feature flag management
- Unit tests

## API Endpoints

### Products
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/products` | List all products |
| GET | `/api/products/{id}` | Get product by ID |
| POST | `/api/products` | Create product (auto-generates Access Keys) |
| PUT | `/api/products/{id}` | Update product name/description/tags |
| DELETE | `/api/products/{id}` | Delete product |
| POST | `/api/products/{id}/rotate-keys` | Rotate Access Keys (Primary→Secondary, new Primary) |

### Environments
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/products/{productId}/environments` | List environments by product |
| GET | `/api/environments/{id}` | Get environment by ID |
| POST | `/api/products/{productId}/environments` | Create environment (auto-generates Access Keys) |
| PUT | `/api/environments/{id}` | Update environment |
| DELETE | `/api/environments/{id}` | Delete environment |
| POST | `/api/environments/{id}/rotate-keys` | Rotate Access Keys |

### Sections
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/environments/{environmentId}/sections` | List sections by environment |
| GET | `/api/sections/{id}` | Get section by ID |
| POST | `/api/environments/{environmentId}/sections` | Create section |
| PUT | `/api/sections/{id}` | Update section |
| DELETE | `/api/sections/{id}` | Delete section |

### Feature Keys
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/sections/{sectionId}/feature-keys` | List feature keys by section |
| GET | `/api/feature-keys/{id}` | Get feature key by ID |
| POST | `/api/sections/{sectionId}/feature-keys` | Create feature key (validates type) |
| PUT | `/api/feature-keys/{id}` | Update feature key (validates type) |
| DELETE | `/api/feature-keys/{id}` | Delete feature key |

## Access Key Security
- Keys generated via `AccessKeyGenerator` using `RandomNumberGenerator` (32 bytes, Base64)
- Rotation: current Primary moves to Secondary, new Primary is generated
- Keys auto-generated on entity creation if not provided

## Feature Key Type Validation
- Strategy pattern: `IFeatureKeyValueValidator` → one implementation per `FeatureKeyType`
- `FeatureKeyValueValidatorFactory` dispatches to the correct validator
- Validators handle both native C# types and `System.Text.Json.JsonElement`
- To add a new type: create validator class implementing `IFeatureKeyValueValidator`, register in `AddFeatureFlagValidators()`
- Validation runs in Provider layer before persistence (`UpsertFeatureKeyAsync`)
- `ValidationRegex` (optional): regex pattern stored on FeatureKey, used by LargeString (validates the whole string) and StringCollection (validates each item)
- Validator signature: `Validate(object? value, string? validationRegex = null)`

## Development Phases

The project follows a 6-stage plan:
1. ~~Data Modeling~~ ✓ - entities, DTOs, repositories
2. ~~Product/Environment CRUD + Access Key rotation~~ ✓
3. ~~Feature Key management + type validators~~ ✓
4. ~~Advanced validations (regex for String Collections)~~ ✓
5. External sources (dynamic fetching from URLs/APIs)
6. Public consumption API (protected by Access Keys, with caching)
