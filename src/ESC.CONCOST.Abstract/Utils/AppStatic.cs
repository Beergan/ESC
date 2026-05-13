using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace ESC.CONCOST.Abstract;

public static class AppStatic
{
    public static IServiceProvider ServiceProvider { get; set; }
    public static ServiceCollection ServiceCollection { get; set; } = new ServiceCollection();
    public static void ReBuildServiceProvider()
    {
        ServiceProvider = ServiceCollection.BuildServiceProvider();
    }
    public static string BaseAddress { get; set; }

    public static async Task<ResultOf<T>> CallApi<T>(Func<Task<ResultOf<T>>> func) where T : class
    {
        try
        {
            return await func();
        }
        catch(Exception ex) 
        {
            return ResultOf<T>.Error($"Mất kết nối đến server|서버 연결 오류 - {ex.Message}");
        }
    }

    public static async Task<ResultsOf<T>> CallApi<T>(Func<Task<ResultsOf<T>>> func) where T : class
    {
        try
        {
            return await func();
        }
        catch (Exception ex)
        {
            return ResultsOf<T>.Error($"Mất kết nối đến server|서버 연결 오류 - {ex.Message}");
        }
    }

    public static async Task<Result> CallApi(Func<Task<Result>> func)
    {
        try
        {
            return await func();
        }
        catch(Exception ex)
        {
            return Result.Error($"Mất kết nối đến server|서버 연결 오류 - {ex.Message}");
        }
    }
}