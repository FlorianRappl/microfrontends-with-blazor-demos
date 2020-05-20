using Microsoft.AspNetCore.Components.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blazor.Sample
{
    public static class Collector
    {
        public static List<Type> MenuItems { get; } = new List<Type>();

        public static List<Assembly> Assemblies { get; } = new List<Assembly>();

        public static void AddLibrary<T>(this IComponentsApplicationBuilder _)
        {
            var assembly = typeof(T).Assembly;
            Assemblies.Add(assembly);
            MenuItems.AddRange(assembly.GetTypes().Where(m => m.Name.Equals("NavItem")));
        }
    }
}
