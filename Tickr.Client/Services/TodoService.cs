using Tickr.Client.Configurations;

namespace Tickr.Client.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Grpc.Core;
    using Helpers;

    public class TodoService 
    {
        private readonly PerformanceSettings _performanceSettings;
        private readonly ITodoClientBuilder _todoClientBuilder;
        private readonly AuthorizationMetadataBuilder _metadataBuilder;

        public TodoService(PerformanceSettings performanceSettings
            , ITodoClientBuilder todoClientBuilder
            , AuthorizationMetadataBuilder metadataBuilder)
        {
            _performanceSettings = performanceSettings;
            _todoClientBuilder = todoClientBuilder;
            _metadataBuilder = metadataBuilder;
        }   
        
        public async Task<List<TodoReply>> GetAllTodo(CancellationToken cancellationToken)
        {
            var client = await _todoClientBuilder.Create();
            var headers = await _metadataBuilder.GetAuthorizationHeader();

            var deadline = DateTime.Now.Add(TimeSpan.FromSeconds(_performanceSettings.DeadlineInMilliseconds)).ToUniversalTime();
            var response =
                client.GetAll(new TodoFilterRequest() {IncludeCompleted = true}, headers, deadline, cancellationToken);

            var replies = new List<TodoReply>();

            while (await response.ResponseStream.MoveNext())
            {
                replies.Add(response.ResponseStream.Current);
            }

            return replies;
        }



        public async Task<CompleteReply> Complete(
            CompleteRequest request
            , CancellationToken cancellationToken = default)
        {
            var client = await _todoClientBuilder.Create();
            var headers = await _metadataBuilder.GetAuthorizationHeader();
            
            var deadline = DateTime.Now.Add(TimeSpan.FromSeconds(_performanceSettings.DeadlineInMilliseconds)).ToUniversalTime();
            var response =
                client.Complete(request, headers, deadline, cancellationToken);

            return response;
        }
    }
}
