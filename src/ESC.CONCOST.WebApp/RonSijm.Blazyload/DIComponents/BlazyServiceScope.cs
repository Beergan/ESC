namespace RonSijm.Blazyload.DIComponents;

public class BlazyServiceScope : IServiceScope
{
    public BlazyServiceScope(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public IServiceProvider ServiceProvider { get; }
}