# EB Feature Flag System

Multitenant feature flag management platform built with .NET Aspire, Blazor Server, and Minimal APIs.

## Architecture

```
Product
├── Environments (Dev, Staging, Production) — each with access keys
├── Sections (logical grouping)
│   └── Feature Flags (definition: name, type, validation)
│       └── Feature Flag Details (per-environment values)
```

- **Products** — Top-level tenants.
- **Environments** — Deployment targets within a product. Each has primary/secondary access keys for SDK authentication.
- **Sections** — Logical groupings of feature flags within a product (e.g. "Authentication", "UI Settings").
- **Feature Flags** — The definition of a flag: name, type, optional validation regex. Belongs to a product and section.
- **Feature Flag Details** — The per-environment value for a feature flag. Automatically created for every environment when a flag is created, and vice versa.

## Tech Stack

| Layer | Technology |
|---|---|
| Orchestration | .NET Aspire |
| API | C# / .NET 10, Minimal APIs |
| Frontend | Blazor Server (.NET 9) |
| Database | Cosmos DB or SQLite |
| Cache | Redis, InMemory, or None |

## Local Setup (SQLite, no cache)

### Prerequisites

- .NET 10 SDK
- .NET Aspire workload (`dotnet workload install aspire`)

### Configuration

The API service reads configuration from `appsettings.json` / `appsettings.Development.json`. For local development with SQLite and no external dependencies:

**`EB.FeatureFlag.Aspire.ApiService/appsettings.Development.json`:**
```json
{
  "FeatureFlag_RepositoryType": "SQLite",
  "FeatureFlag_RepositoryConnectionString": "Data Source=featureflag.db",
  "FeatureFlag_CacheType": "None",
  "FeatureFlag_CacheConnectionString": ""
}
```

| Setting | Options | Notes |
|---|---|---|
| `FeatureFlag_RepositoryType` | `SQLite`, `Cosmos` | SQLite for local, Cosmos for production |
| `FeatureFlag_RepositoryConnectionString` | Connection string | SQLite: `Data Source=featureflag.db` |
| `FeatureFlag_CacheType` | `None`, `InMemory`, `Redis` | `None` skips caching entirely |
| `FeatureFlag_CacheConnectionString` | Connection string | Only needed for Redis |

### Build & Run

```bash
# Build the solution
dotnet build EB.FeatureFlag.Aspire.sln

# Run via Aspire (starts API + Web frontend)
dotnet run --project EB.FeatureFlag.Aspire.AppHost
```

The Aspire dashboard will open and show both services. The SQLite database file (`featureflag.db`) is auto-created on first run.

## UI Guide

The Blazor web frontend is available at the URL shown in the Aspire dashboard (typically `https://localhost:7000`).

### Navigation

The sidebar provides access to all main areas:

- **Home** — Dashboard with counts for products, environments, sections, and feature flags. Includes SDK integration instructions.
- **Products** — Create and manage products. Each product row links to its environments, sections, and feature flags.
- **Environments** — Manage environments per product. Access keys are generated automatically and can be rotated. Accessible standalone (`/environments` with product selector) or via product (`/products/{id}/environments`).
- **Sections** — Manage sections per product. Each section row links to its feature flags. Accessible standalone (`/sections`) or via product (`/products/{id}/sections`).
- **Feature Flags** — Create and manage feature flags. Accessible standalone (`/feature-flags`), via product (`/products/{id}/feature-flags`), or via section (`/sections/{id}/feature-flags` — auto-selects product and section filter).

### Creating a Feature Flag

1. Navigate to **Feature Flags** and select a product (or arrive from a product/section link).
2. Click **+ New Feature Flag**.
3. Fill in: Name, Section, Type, and optionally Description, Tags, Validation Regex.
4. Save. The system automatically creates a detail record for every environment in the product.

### Setting Per-Environment Values

1. On the Feature Flags list, click **Details** on any flag.
2. A modal shows one row per environment. Click **Edit** on any row to set the value.
3. Save. Each environment can have a different value for the same feature flag.

If a new environment is added after the flag was created, the detail record is auto-created when you open the Details modal.

### Feature Flag Types & Value Formats

| Type | Description | Value Format |
|---|---|---|
| **Boolean** | On/off toggle | `true` or `false` (dropdown in UI) |
| **LargeString** | Free-text string | Any text value |
| **StringCollection** | List of strings | JSON array: `["value1", "value2", "value3"]` |
| **JsonCollection** | List of JSON objects | JSON array: `[{"key": "value"}, {"other": 123}]` |

#### StringCollection Examples

```json
["alpha", "beta", "gamma"]
["enabled-feature-a", "enabled-feature-b"]
```

If a `ValidationRegex` is set on the flag, each string in the array is validated against it.

#### JsonCollection Examples

```json
[{"key": "value"}, {"other": 123}]
[["nested", "arrays"], ["are", "allowed"]]
```

Each element must be a valid JSON object, array, or JSON string.

### Environment Access Keys

- Access keys are auto-generated when an environment is created (primary + secondary).
- Use **Rotate Keys** to regenerate: the current primary becomes secondary, and a new primary is generated.
- Keys are used to authenticate SDK API requests.

## API Reference

### Management Endpoints

#### Products

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/products` | List all products |
| GET | `/api/products/{id}` | Get product by ID |
| POST | `/api/products` | Create product |
| PUT | `/api/products/{id}` | Update product |
| DELETE | `/api/products/{id}` | Delete product |

#### Environments

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/products/{productId}/environments` | List environments by product |
| GET | `/api/environments/{id}` | Get environment by ID |
| POST | `/api/products/{productId}/environments` | Create environment |
| PUT | `/api/environments/{id}` | Update environment |
| DELETE | `/api/environments/{id}` | Delete environment |
| POST | `/api/environments/{id}/rotate-keys` | Rotate access keys |

#### Sections

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/products/{productId}/sections` | List sections by product |
| GET | `/api/sections/{id}` | Get section by ID |
| POST | `/api/products/{productId}/sections` | Create section |
| PUT | `/api/sections/{id}` | Update section |
| DELETE | `/api/sections/{id}` | Delete section |

#### Feature Flags

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/products/{productId}/feature-flags` | List flags by product |
| GET | `/api/sections/{sectionId}/feature-flags` | List flags by section |
| GET | `/api/feature-flags/{id}` | Get flag by ID |
| POST | `/api/sections/{sectionId}/feature-flags` | Create flag (auto-creates details per environment) |
| PUT | `/api/feature-flags/{id}` | Update flag definition |
| DELETE | `/api/feature-flags/{id}` | Delete flag (cascades to details) |

#### Feature Flag Details

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/feature-flags/{featureFlagId}/details` | List all details for a flag |
| GET | `/api/feature-flag-details/{id}` | Get detail by ID |
| PUT | `/api/feature-flag-details/{id}` | Update detail value |

### SDK Endpoint

> **Note:** SDK endpoint design is subject to change.

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/sdk/feature-flags` | Get all feature flags for an environment |

**Required Header:**

| Header | Description |
|---|---|
| `X-Environment-Key` | Primary or secondary access key of the environment |

**Example Request:**

```
GET /api/sdk/feature-flags
X-Environment-Key: your-environment-access-key
```

**Example Response:**

```json
{
  "product": "MyProduct",
  "environment": "Production",
  "sections": [
    {
      "name": "Authentication",
      "flags": [
        { "name": "EnableSSO", "type": 0, "value": true },
        { "name": "AllowedProviders", "type": 2, "value": ["Google", "GitHub"] }
      ]
    }
  ]
}
```

The response is cached for 5 minutes per product+environment combination.
