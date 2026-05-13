using Microsoft.Extensions.DependencyInjection;
using System;

namespace ESC.CONCOST.Abstract;

public static class StaticResolver
{
    private static IServiceProvider _serviceProvider;

    public static void MakeMeCanResolveFromStatic(this IServiceProvider sp)
    {
        _serviceProvider = sp;
    }

    public static T Resolve<T>()
    {
        return _serviceProvider.GetRequiredService<T>();
    }

    public static bool TryResolve<T>(out T service)
    {
        try
        {
            service = _serviceProvider.GetService<T>();
            return true;
        }
        catch (Exception)
        {
            service = default(T);
            return false;
        }
    }
}