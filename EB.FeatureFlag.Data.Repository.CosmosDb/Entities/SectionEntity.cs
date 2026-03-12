using System;
using System.Collections.Generic;

namespace EB.FeatureFlag.Data.Repository.CosmosDb.Entities;

public class SectionEntity
{
    public Guid Id { get; set; }
    public Guid EnvironmentId { get; set; }
    public Guid ProductId { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public List<string>? Tags { get; set; }
}