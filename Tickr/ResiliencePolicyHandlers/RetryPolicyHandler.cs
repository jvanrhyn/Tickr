namespace Tickr.ResiliencePolicyHandlers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Polly;

    public class RetryPolicyHandler
    {
        private readonly ILogger<RetryPolicyHandler> _logger;

        public RetryPolicyHandler(ILogger<RetryPolicyHandler> logger)
        {
            _logger = logger;
        }

        public async Task<T> Retry<T>(int retryCount, Func<Task<T>> function)
        {
            var nameOfFunc = function.Method.Name;
            return await Policy
                .Handle<Exception>()
                .RetryAsync(retryCount, async (exception, count) =>
                {
                    await Task.Delay(1000 * retryCount).ConfigureAwait(false);
                    _logger.LogWarning("Retrying operation {functionName}. {count}/{retryCount} : Reason : '{message}'"
                        , nameOfFunc
                        , count
                        , retryCount
                        , exception.GetBaseException().Message);
                })
                .ExecuteAsync(async () => await function())
                .ConfigureAwait(false);
        }
    }
}