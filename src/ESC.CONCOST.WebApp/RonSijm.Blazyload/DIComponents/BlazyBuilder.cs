namespace RonSijm.Blazyload.DIComponents;

public class BlazyBuilder(IServiceCollection services)
{
    public IServiceProvider GetServiceProvider()
    {
        var serviceProvider = new BlazyServiceProvider(services);

        return serviceProvider;
    }
}