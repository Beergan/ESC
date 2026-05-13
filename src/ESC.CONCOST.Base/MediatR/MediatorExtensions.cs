using System;
using Hangfire;
using MediatR;
using Newtonsoft.Json;

namespace ESC.CONCOST.Base;

public static class MediatorExtensions
{
    public static string SendNow(this IMediator mediator, IRequest request, string description = null)
    {
        var mediatorSerializedObject = SerializeObject(request, description);
        return BackgroundJob.Enqueue<IMediatorHangfireBridge>(b => b.Send(mediatorSerializedObject));
    }

    [Obsolete]
    public static string SendNow(this IMediator mediator, IRequest request, string parentJobId, JobContinuationOptions continuationOption, string description = null)
    {
        var mediatorSerializedObject = SerializeObject(request, description);
        return BackgroundJob.ContinueWith<IMediatorHangfireBridge>(parentJobId, b => b.Send(mediatorSerializedObject), continuationOption);
    }

    public static void Schedule(this IMediator mediator, IRequest request, DateTimeOffset scheduleAt, string description = null)
    {
        var mediatorSerializedObject = SerializeObject(request, description);
        BackgroundJob.Schedule<IMediatorHangfireBridge>(b => b.Send(mediatorSerializedObject), scheduleAt);
    }
    
    public static void Schedule(this IMediator mediator, IRequest request, TimeSpan delay, string description = null)
    {
        var mediatorSerializedObject = SerializeObject(request, description);
        var newTime = DateTime.UtcNow + delay;
        BackgroundJob.Schedule<IMediatorHangfireBridge>(b => b.Send(mediatorSerializedObject), newTime);
    }

    [Obsolete]
    public static void ScheduleRecurring(this IMediator mediator, IRequest request, string name, string cronExpression, string description = null)
    {
        var mediatorSerializedObject = SerializeObject(request, description);
        RecurringJob.AddOrUpdate<IMediatorHangfireBridge>(name, b => b.Send(mediatorSerializedObject), cronExpression, TimeZoneInfo.Local);
    }

    private static MediatorSerializedObject SerializeObject(object mediatorObject, string description)
    {
        string fullTypeName = mediatorObject.GetType().FullName;
        string data = JsonConvert.SerializeObject(mediatorObject, new JsonSerializerSettings
        {
            Formatting = Formatting.None,
        });

        return new MediatorSerializedObject(fullTypeName, data, description);
    }
}