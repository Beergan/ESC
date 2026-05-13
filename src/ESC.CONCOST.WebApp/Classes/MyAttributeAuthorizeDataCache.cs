using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;

namespace Microsoft.AspNetCore.Components.Authorization;

internal static class MyAttributeAuthorizeDataCache
{
    private static ConcurrentDictionary<Type, IAuthorizeData[]> _cache = new ConcurrentDictionary<Type, IAuthorizeData[]>();

    public static IAuthorizeData[] GetAuthorizeDataForType(Type type)
    {
        IAuthorizeData[] result;
        if (!_cache.TryGetValue(type, out result))
        {
            result = ComputeAuthorizeDataForType(type);
            _cache[type] = result;
        }

        return result;
    }

    private static IAuthorizeData[] ComputeAuthorizeDataForType(Type type)
    {
        var allAttributes = type.GetCustomAttributes(inherit: true);
        if (allAttributes.OfType<IAllowAnonymous>().Any())
        {
            return null;
        }

        var authorizeDataAttributes = allAttributes.OfType<IAuthorizeData>().ToArray();
        return authorizeDataAttributes.Length > 0 ? authorizeDataAttributes : null;
    }
}