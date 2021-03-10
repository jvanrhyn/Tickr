namespace Tickr.ResiliencePolicyHandlers
{
    using System;
    using System.Threading;
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

        public int RetryCounter { get; set; }

        public async Task<T> Retry<T>(int retryCount, Func<Task<T>> function, CancellationToken cancellationToken = default)
        {
            var nameOfFunc = function.Method.Name;
            return await Policy
                .Handle<Exception>()
                .RetryAsync(retryCount, async (exception, count) =>
                {
                    this.RetryCounter = count;
                    var waitTime = Math.Pow(2, retryCount);
                    await Task.Delay((int)waitTime, cancellationToken).ConfigureAwait(false);
                    _logger.LogWarning("Retrying operation {functionName}. {count}/{retryCount} : Reason : '{message}'"
                        , nameOfFunc
                        , count
                        , retryCount
                        , exception.Message);
                })
                .ExecuteAsync(async () => await function())
                .ConfigureAwait(false);
        }
    }
}