using System;
using System.Collections.Generic;

namespace Blazor.Sample
{
    public interface IComponentActivationService
    {
        event EventHandler Changed;

        IEnumerable<ActiveComponent> Components { get; }
    }
}
