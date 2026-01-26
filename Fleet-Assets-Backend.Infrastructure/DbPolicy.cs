using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Timeout;

namespace Fleet_Assets_Backend.Infrastructure;

public static class DbPolicies
{
    public static IAsyncPolicy CreateReadPolicy(TimeSpan perTryTimeout, int retries, ILogger logger)
    {
        var timeout = Policy.TimeoutAsync(perTryTimeout, TimeoutStrategy.Optimistic);

        bool IsSqlCancel(SqlException ex) =>
            ex.Message.Contains("Operation cancelled by user", StringComparison.OrdinalIgnoreCase);

        var retry = Policy
            .Handle<TimeoutRejectedException>()
            .Or<TaskCanceledException>()
            .Or<SqlException>(IsSqlCancel)
            .RetryAsync(retries, (ex, attempt) =>
            {
                logger.LogWarning(ex, "DB retry attempt {Attempt} after timeout/cancel.", attempt);
                return Task.CompletedTask;
            });

        return Policy.WrapAsync(retry, timeout);
    }
}