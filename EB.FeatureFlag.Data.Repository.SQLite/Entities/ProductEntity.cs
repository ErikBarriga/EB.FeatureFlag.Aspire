using System;
using System.Collections.Generic;

namespace EB.FeatureFlag.Data.Repository.SQLite.Entities;

public class ProductEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public List<string>? Tags { get; set; }
    public string PrimaryAccessKey { get; set; } = default!;
    public string SecondaryAccessKey { get; set; } = default!;
}
