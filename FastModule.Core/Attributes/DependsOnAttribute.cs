namespace FastModule.Core.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DependsOnAttribute(Type moduleType) : Attribute
{
    public Type ModuleType { get; } = moduleType;
}
