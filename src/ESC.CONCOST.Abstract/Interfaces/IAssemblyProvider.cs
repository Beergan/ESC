using System.Reflection;

namespace ESC.CONCOST.Abstract;

public interface IAssemblyProvider
{
    Assembly[] GetAssemblies();
}