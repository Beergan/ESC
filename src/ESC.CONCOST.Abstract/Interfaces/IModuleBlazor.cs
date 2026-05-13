using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace ESC.CONCOST.Abstract;

public interface IModuleBlazor
{
    //void SetUpMenu(IDictionary<string, List<(string, string)>> menu);

    void ConfigureServices(IServiceCollection services);
}