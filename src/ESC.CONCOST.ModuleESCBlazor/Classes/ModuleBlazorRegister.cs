using Microsoft.Extensions.DependencyInjection;
using RestEase.HttpClientFactory;
using ESC.CONCOST.Abstract;
using ESC.CONCOST.ModuleESCCore;
using ESC.CONCOST.ModuleESCCore.Interfaces;

namespace ESC.CONCOST.ModuleESCBlazor;

public class ModuleBlazorRegister : IModuleBlazor
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRestEaseClient<IESCService>(AppStatic.BaseAddress);
        services.AddRestEaseClient<IConstructionCategoryService>(AppStatic.BaseAddress);
    }
}