using ESC.CONCOST.Base;
using ESC.CONCOST.ModuleSettingCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ESC.CONCOST.ModuleSetting;

public class ModuleAspNetRegister : IModuleAspNet
{
    public void BuildModule(IApplicationBuilder app)
    {
        GlobalPermissions.Register(typeof(PERMISSION));
    }

    public void ConfigureServices(IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IEscFormulaSettingService, EscFormulaSettingService>();
    }
}