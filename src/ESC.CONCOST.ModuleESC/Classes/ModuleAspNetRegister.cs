using ESC.CONCOST.Abstract;
using ESC.CONCOST.Base;
using ESC.CONCOST.ModuleESC.Services;
using ESC.CONCOST.ModuleESCCore;
using ESC.CONCOST.ModuleESCCore.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RestEase.HttpClientFactory;
using System.Linq;
using System.Security;

namespace ESC.CONCOST.ModuleESC;

public class ModuleAspNetRegister : IModuleAspNet
{
    public void BuildModule(IApplicationBuilder app)
    {
        GlobalPermissions.Register(typeof(PERMISSION));
    }

    public void ConfigureServices(IServiceCollection services, Microsoft.Extensions.Configuration.IConfiguration config)
    {
        services.AddScoped<IESCService, ESCService>();
        services.AddScoped<IConstructionCategoryService, ConstructionCategoryService>();
        services.AddScoped<IContractCategoryService, ContractCategoryService>();
        services.AddScoped<ICustomerAdminService, CustomerAdminService>();
    }
}