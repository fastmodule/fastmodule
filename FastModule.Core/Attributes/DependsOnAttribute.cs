namespace FastModule.Core.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DependsOnAttribute(params Type[] dependencies) : Attribute
{
    public Type[] Dependencies { get; } = dependencies;
}