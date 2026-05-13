using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using ESC.CONCOST.Abstract;

namespace ESC.CONCOST.Base;

public interface IMediatorHangfireBridge
{
    Task Send(MediatorSerializedObject mediatorSerializedObject);
}

public class MediatorHangfireBridge : IMediatorHangfireBridge
{
    private readonly IMediator _mediator;

    public MediatorHangfireBridge(IMediator mediator)
    {
        _mediator = mediator;
    }

    [DisplayName("{0}")]
    public Task Send(MediatorSerializedObject mediatorSerializedObject)
    {
        var assemblies = AssembliesUtil.GetAspNetAssemblies();
        var assemblyName = mediatorSerializedObject.FullTypeName.GetBeforeLast(".") + ".dll";
        var assembly = assemblies.Single(x => x.ManifestModule.Name == assemblyName);
        var type = assembly.GetType(mediatorSerializedObject.FullTypeName);
        if (type == null) return null;

        dynamic req = JsonConvert.DeserializeObject(mediatorSerializedObject.Data, type);
        return _mediator.Send(req as IRequest);
    }
}