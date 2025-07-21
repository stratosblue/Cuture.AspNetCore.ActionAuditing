namespace SampleFullAuditWebApp.Auditing;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class FeatureNameAttribute(string featureName) : Attribute
{
    public string FeatureName { get; } = featureName;
}
