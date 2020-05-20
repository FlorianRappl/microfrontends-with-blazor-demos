using Blazor.Sample.Shared;
using System;

namespace Blazor.Sample
{
    public readonly struct ActiveComponent
    {
        public ActiveComponent(string componentName, string referenceId, Type component)
        {
            ComponentName = componentName ?? string.Empty;
            ReferenceId = referenceId ?? string.Empty;
            Component = component ?? typeof(Empty);
        }

        public string ComponentName { get; }

        public string ReferenceId { get; }

        public Type Component { get; }
    }
}