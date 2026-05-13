// ReSharper disable global EventNeverSubscribedTo.Global - Justification: Used by library consumers
// ReSharper disable global UnusedMember.Global - Justification: Used by library consumers

using BlazorApp1.Client.RonSijm.Blazyload.DIComponents;

namespace RonSijm.Blazyload.DIComponents;

public class WasmServiceRegister(BlazyServiceProvider blazyServiceProvider) : IWasmServiceRegister
{
    public void Register(IEnumerable<ServiceDescriptor> services)
    {
        blazyServiceProvider.Register(services);
    }
}