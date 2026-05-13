using RestEase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESC.CONCOST.Abstract;

[BasePath("api/notifier")]
public interface INotifyService
{
    [Post(nameof(AddNotify))]
    Task<Result> AddNotify(string content, string href, Guid? targetUserGuid = null);

    [Get(nameof(GetNotifies))]
    Task<ResultsOf<ModelNotification>> GetNotifies([Query] Guid? userGuid = null);
}