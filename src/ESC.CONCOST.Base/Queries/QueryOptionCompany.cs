using System;
using System.Collections.Generic;
using MediatR;
using ESC.CONCOST.Abstract;

namespace ESC.CONCOST.Base;

public class QueryOptionCompany : IRequest<OptionItem<Guid>>
{
}