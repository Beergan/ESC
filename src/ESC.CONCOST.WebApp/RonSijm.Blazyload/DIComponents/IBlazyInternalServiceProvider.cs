namespace RonSijm.Blazyload.DIComponents;

public interface IBlazyInternalServiceProvider
{
    bool TryGetServiceFromOverride(Type serviceType, out object value);
}