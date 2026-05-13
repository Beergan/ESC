using System;
using System.Threading.Tasks;

namespace ESC.CONCOST.Abstract;

public interface IServiceInvoker
{
    Task<ResultOf<T>> CallApi<T>(Func<Task<ResultOf<T>>> func) where T : class;

    Task<ResultsOf<T>> CallApi<T>(Func<Task<ResultsOf<T>>> func) where T : class;

    Task<Result> CallApi(Func<Task<Result>> func);
}