using Microsoft.JSInterop;
using ESC.CONCOST.Abstract;

namespace ESC.CONCOST.WebApp;

public class SweetAlertWasmService : ISweetAlertService
{
    private readonly IJSRuntime js;
    private static string prefix = "swal_driver";

    public SweetAlertWasmService(IJSRuntime jSRuntime)
    {
        this.js = jSRuntime;
    }

    public Task Close()
    {
        return js.InvokeVoidAsync($"{prefix}.{nameof(Close)}").AsTask();
    }

    public  Task Loading(string message, string header = null)
    {
        return js.InvokeVoidAsync($"{prefix}.{nameof(Loading)}", header, message).AsTask();
    }

    public Task Alert(string message, string header = null)
    {
        return js.InvokeVoidAsync($"{prefix}.{nameof(Alert)}", header, message).AsTask();
    }

    public Task Error(string message, string header = null)
    {
        return js.InvokeVoidAsync($"{prefix}.{nameof(Error)}", header, message).AsTask();
    }

    public Task<bool> ConfirmDelete(string message, string header = null)
    {
        return InvokeValue<bool>(nameof(ConfirmDelete), header, message);
    }

    public Task<T> InvokeValue<T>(string method, params object[] args)
    {
        var tcs = new TaskCompletionSource<T>();
        var promiseHandler = DotNetObjectReference.Create<JsPromiseHandler<T>>(new JsPromiseHandler<T>() { tcs = tcs });

        js.InvokeAsync<object>($"{prefix}.invokeAsPromise", promiseHandler, method, args);

        return tcs.Task;
    }
}