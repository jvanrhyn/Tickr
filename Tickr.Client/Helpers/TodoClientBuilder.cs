namespace Tickr.Client.Helpers
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using Configurations;
    using Grpc.Net.Client;

    public class TodoClientBuilder : ITodoClientBuilder
    {

        private readonly ServerSettings _serverSettings;

        public TodoClientBuilder(ServerSettings serverSettings)
        {
            _serverSettings = serverSettings;
        }

        public Task<Todo.TodoClient> Create()
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

            return Task.FromResult(client);
        }
    }
}