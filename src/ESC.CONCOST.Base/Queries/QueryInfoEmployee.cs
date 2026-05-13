using System;
using MediatR;
using ESC.CONCOST.Abstract;

namespace ESC.CONCOST.Base;

public class QueryInfoEmployee : IRequest<ModelInfoEmployee>
{
    public Guid Guid { get; set; }
}