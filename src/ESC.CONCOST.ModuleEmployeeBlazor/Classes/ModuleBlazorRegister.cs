using Microsoft.Extensions.DependencyInjection;
using RestEase.HttpClientFactory;
using ESC.CONCOST.Abstract;
using ESC.CONCOST.ModuleEmployeeCore;

namespace ESC.CONCOST.ModuleEmployeeBlazor;

public class ModuleBlazorRegister : IModuleBlazor
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRestEaseClient<IEmployeeService>(AppStatic.BaseAddress);
    }
}