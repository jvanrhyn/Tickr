namespace Tickr.Client.Services
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Cofigurations;
    using Grpc.Core;
    using Grpc.Net.Client;
    using Microsoft.Extensions.Configuration;
    using Helpers;

    public class TodoService 
    {
        private readonly IAuthorizationHelper _authorizationHelper;
        private readonly PerformanceSettings _performanceSettings;
        private readonly ServerSettings _serverSettings;

        public TodoService(IAuthorizationHelper authorizationHelper, PerformanceSettings performanceSettings, ServerSettings serverSettings)
        {
            _authorizationHelper = authorizationHelper;
            _performanceSettings = performanceSettings;
            _serverSettings = serverSettings;
        }   
        
        public async Task<List<TodoReply>> GetAllTodo(CancellationToken cancellationToken)
        {
            var serverAddress = _serverSettings.TodoServerHttps;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                AppContext.SetSwitch(
                    "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                serverAddress = _serverSettings.TodoServerHttp;
            }
            
            var channel = GrpcChannel.ForAddress(serverAddress);
            var client = new Todo.TodoClient(channel);

            var accessToken = await _authorizationHelper.GetAccessToken();
            var headers = new Metadata {{"Authorization", $"Bearer {accessToken}"}};


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
        
        
       
    }
}
