namespace BlazorApp1.Client.RonSijm.Blazyload.DIComponents;

public interface IWasmServiceRegister
{
    void Register(IEnumerable<ServiceDescriptor> services);
}