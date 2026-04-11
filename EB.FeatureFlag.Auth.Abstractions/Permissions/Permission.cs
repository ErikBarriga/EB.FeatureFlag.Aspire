namespace EB.FeatureFlag.Auth.Abstractions.Permissions;

[Flags]
public enum Permission
{
    None = 0,

    ProductRead = 1 << 0,
    ProductWrite = 1 << 1,
    ProductDelete = 1 << 2,

    EnvironmentRead = 1 << 3,
    EnvironmentWrite = 1 << 4,
    EnvironmentDelete = 1 << 5,
    EnvironmentRotateKeys = 1 << 6,

    SectionRead = 1 << 7,
    SectionWrite = 1 << 8,
    SectionDelete = 1 << 9,

    FeatureFlagRead = 1 << 10,
    FeatureFlagWrite = 1 << 11,
    FeatureFlagDelete = 1 << 12,

    UserManagement = 1 << 13,

    AllRead = ProductRead | EnvironmentRead | SectionRead | FeatureFlagRead,
    AllWrite = ProductWrite | EnvironmentWrite | SectionWrite | FeatureFlagWrite,
    AllDelete = ProductDelete | EnvironmentDelete | SectionDelete | FeatureFlagDelete,
    All = AllRead | AllWrite | AllDelete | EnvironmentRotateKeys | UserManagement
}
