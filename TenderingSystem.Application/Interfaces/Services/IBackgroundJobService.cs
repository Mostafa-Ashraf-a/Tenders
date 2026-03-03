namespace TenderingSystem.Application.Interfaces.Services;

public interface IBackgroundJobService
{
    /// <summary>
    /// Enqueues a fire-and-forget background job.
    /// </summary>
    string Enqueue(System.Linq.Expressions.Expression<Func<Task>> methodCall);

    /// <summary>
    /// Enqueues a fire-and-forget background job with a resolved dependency.
    /// </summary>
    string Enqueue<T>(System.Linq.Expressions.Expression<Func<T, Task>> methodCall);

    /// <summary>
    /// Adds or updates a recurring background job.
    /// </summary>
    void AddOrUpdateRecurringJob(string recurringJobId, System.Linq.Expressions.Expression<Func<Task>> methodCall, string cronExpression);

    /// <summary>
    /// Removes a recurring background job.
    /// </summary>
    void RemoveRecurringJob(string recurringJobId);
}
