using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ESC.CONCOST.Abstract;

public static class AssembliesUtil
{
    private static List<Assembly> allAssemblies = null;

    public static IEnumerable<Assembly> GetAssemblies()
    {
        if (allAssemblies == null)
        {
            var modules = new List<Assembly>();
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var files = Directory.GetFiles(path, "*.dll");

            foreach (string dll in files.Where(x => Path.GetFileName(x).StartsWith("ESC.CONCOST.Module")))
            {
                var name = dll.GetAfterLast("\\").GetBeforeLast(".");
                if (modules.Any(t => t.GetName().Name == name))
                    continue;

                modules.Add(Assembly.LoadFile(dll));
            }

            allAssemblies = modules;
        }

        return allAssemblies;
    }

    public static IEnumerable<Assembly> GetBlazorAssemblies()
    {
        return GetAssemblies().Where(a => a.GetName().Name.EndsWith("Blazor"));
    }

    public static IEnumerable<Assembly> GetAspNetAssemblies()
    {
        return GetAssemblies().Where(a =>
                !a.GetName().Name.EndsWith("Blazor")
            && !a.GetName().Name.EndsWith("Core")
        );
    }

    public static IEnumerable<TypeInfo> GetTypes<T>(this IEnumerable<Assembly> assemblies)
    {
        return assemblies.SelectMany(a => a.DefinedTypes.Where(x => x.GetInterfaces().Contains(typeof(T))));
    }

    public static IEnumerable<T> GetInstances<T>(this IEnumerable<Assembly> assemblies)
    {
        var instances = new List<T>();

        foreach (Type implementation in assemblies.GetTypes<T>())
        {
            if (implementation.GetTypeInfo().IsAbstract)
                continue;

            var instance = (T)Activator.CreateInstance(implementation);
            instances.Add(instance);
        }

        return instances;
    }
}