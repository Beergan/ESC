using Microsoft.JSInterop;
using ESC.CONCOST.Abstract;
using ESC.CONCOST.WebApp;

namespace ESC.CONCOST.WebHost;

public class SweetAlertServerService : ISweetAlertService
{
    private readonly IJSRuntime js;
    private readonly IHttpContextAccessor httpContextAccessor;
    private static string prefix = "swal_driver";
    private bool IsPreRendering => httpContextAccessor?.HttpContext?.Response?.HasStarted != true;

    public SweetAlertServerService(IJSRuntime jSRuntime, IHttpContextAccessor ctxAccessor)
    {
        js = jSRuntime;
        httpContextAccessor = ctxAccessor;
    }

    public Task Close()
    {
        if(IsPreRendering) return Task.CompletedTask;
        return js.InvokeVoidAsync($"{prefix}.{nameof(Close)}").AsTask();
    }

    public  Task Loading(string message, string header = null)
    {
        if(IsPreRendering) return Task.CompletedTask;
        return js.InvokeVoidAsync($"{prefix}.{nameof(Loading)}", header, message).AsTask();
    }

    public Task Alert(string message, string header = null)
    {
        if (IsPreRendering) return Task.CompletedTask;
        return js.InvokeVoidAsync($"{prefix}.{nameof(Alert)}", header, message).AsTask();
    }

    public Task Error(string message, string header = null)
    {
        if (IsPreRendering) return Task.CompletedTask;
        return js.InvokeVoidAsync($"{prefix}.{nameof(Error)}", header, message).AsTask();
    }

    public Task<bool> ConfirmDelete(string message, string header = null)
    {
        return InvokeValue<bool>(nameof(ConfirmDelete), header, message);
    }

    public Task<T> InvokeValue<T>(string method, params object[] args)
    {
        if (IsPreRendering) return Task.FromResult(default(T));
        var tcs = new TaskCompletionSource<T>();
        var promiseHandler = DotNetObjectReference.Create<JsPromiseHandler<T>>(new JsPromiseHandler<T>() { tcs = tcs });

        js.InvokeAsync<object>($"{prefix}.invokeAsPromise", promiseHandler, method, args);

        return tcs.Task;
    }
}