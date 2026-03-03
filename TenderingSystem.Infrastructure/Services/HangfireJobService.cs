using Hangfire;
using TenderingSystem.Application.Interfaces.Services;

namespace TenderingSystem.Infrastructure.Services;

public class HangfireJobService : IBackgroundJobService
{
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IRecurringJobManager _recurringJobManager;

    public HangfireJobService(IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager)
    {
        _backgroundJobClient = backgroundJobClient;
        _recurringJobManager = recurringJobManager;
    }

    public string Enqueue(System.Linq.Expressions.Expression<Func<Task>> methodCall)
    {
        return _backgroundJobClient.Enqueue(methodCall);
    }

    public string Enqueue<T>(System.Linq.Expressions.Expression<Func<T, Task>> methodCall)
    {
        return _backgroundJobClient.Enqueue<T>(methodCall);
    }

    public void AddOrUpdateRecurringJob(string recurringJobId, System.Linq.Expressions.Expression<Func<Task>> methodCall, string cronExpression)
    {
        _recurringJobManager.AddOrUpdate(recurringJobId, methodCall, cronExpression);
    }

    public void RemoveRecurringJob(string recurringJobId)
    {
        _recurringJobManager.RemoveIfExists(recurringJobId);
    }
}
