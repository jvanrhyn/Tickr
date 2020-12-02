namespace Tickr.Client.Services
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using Grpc.Core;
    using Grpc.Net.Client;
    using Microsoft.Extensions.Configuration;
    using Tickr.Client.Helpers;

    public class TodoService 
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthorizationHelper _authorizationHelper;

        public TodoService(IConfiguration configuration, IAuthorizationHelper authorizationHelper)
        {
            _configuration = configuration;
            _authorizationHelper = authorizationHelper;
        }   
        
        public async Task<List<TodoReply>> GetAllTodo()
        {
            var serverAddress = "https://localhost:5001";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                AppContext.SetSwitch(
                    "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                serverAddress = "http://localhost:5000";
            }
            
            var channel = GrpcChannel.ForAddress(serverAddress);
            var client = new Todo.TodoClient(channel);

            var accessToken = await _authorizationHelper.GetAccessToken();
            var headers = new Metadata {{"Authorization", $"Bearer {accessToken}"}};


            var response = 
                client.GetAll(new TodoFilterRequest() {IncludeCompleted = true}, headers);

            var replies = new List<TodoReply>();

            while (await response.ResponseStream.MoveNext())
            {
                replies.Add(response.ResponseStream.Current);
            }

            return replies;
        }
        
        
       
    }
}