using BlazorApp1.Client.RonSijm.Blazyload.DIComponents;

namespace RonSijm.Blazyload.DIComponents;

public class BlazyServiceProviderFactory : IServiceProviderFactory<BlazyBuilder>
{
    public BlazyServiceProviderFactory()
    {
    }

    public BlazyBuilder CreateBuilder(IServiceCollection services)
    {
        services.AddSingleton<IWasmServiceRegister, WasmServiceRegister>();
        var container = new BlazyBuilder(services);
        return container;
    }

    public IServiceProvider CreateServiceProvider(BlazyBuilder blazyBuilder)
    {
        var serviceProvider = blazyBuilder.GetServiceProvider();

        return serviceProvider;
    }
}