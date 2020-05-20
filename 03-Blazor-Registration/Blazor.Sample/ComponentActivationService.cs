using System;
using System.Collections.Generic;

namespace Blazor.Sample
{
    public class ComponentActivationService : IComponentActivationService
    {
        private readonly Dictionary<string, Type> _services = new Dictionary<string, Type>();
        private readonly List<ActiveComponent> _active = new List<ActiveComponent>();

        public event EventHandler Changed;

        public IEnumerable<ActiveComponent> Components => _active;

        public void Register(string componentName, Type componentType)
        {
            if (_services.ContainsKey(componentName))
            {
                throw new InvalidOperationException("The provided component name has already been registered.");
            }

            _services.Add(componentName, componentType);
        }

        public void ActivateComponent(string componentName, string referenceId)
        {
            _active.Add(new ActiveComponent(componentName, referenceId, GetComponent(componentName)));
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void DeactivateComponent(string componentName, string referenceId)
        {
            var removed = _active.RemoveAll(m => m.ComponentName == componentName && m.ReferenceId == referenceId);

            if (removed > 0)
            {
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        private Type GetComponent(string componentName)
        {
            _services.TryGetValue(componentName, out var value);
            return value;
        }
    }
}
