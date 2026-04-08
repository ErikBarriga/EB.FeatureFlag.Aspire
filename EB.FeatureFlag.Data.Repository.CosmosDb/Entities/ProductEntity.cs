using System;
using System.Collections.Generic;

namespace EB.FeatureFlag.Data.Repository.CosmosDb.Entities;

public class ProductEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public List<string>? Tags { get; set; }
}
