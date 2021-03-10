namespace Tickr.Tests.Unit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging.Abstractions;
    using ResiliencePolicyHandlers;
    using Shouldly;
    using Xunit;

    public class PolicyTests
    {
        [Fact]
        public async Task CanRetryTwiceBeforeSuccess()
        {
            var i = 0;
            RetryPolicyHandler retryPolicyHandler = new(NullLogger<RetryPolicyHandler>.Instance);
            var result = await retryPolicyHandler.Retry<int>(3,  () =>
            {
                i++;
                if (i % 3 != 0)
                {
                    throw new Exception($"Should retry again :  {i}");
                }

                return Task.FromResult(i);
            }, CancellationToken.None);

            result.ShouldBe(3);
            retryPolicyHandler.RetryCounter.ShouldBe(2);
        }

        [Fact]
        public async Task RetryReturnsExceptionAfterThreeFails()
        {
            var i = 0;
            RetryPolicyHandler retryPolicyHandler = new(NullLogger<RetryPolicyHandler>.Instance);

            await Assert.ThrowsAsync<Exception>(async () =>
            {
                var result = await retryPolicyHandler.Retry<int>(3, () =>
                {
                    i++;
                    throw new Exception();
                }, CancellationToken.None);
            });
        }
    }
}