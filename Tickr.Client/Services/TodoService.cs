using Grpc.Core;

namespace Tickr.Client.Services
{
    public class TodoService : Todo.TodoClient
    {
        public override AsyncServerStreamingCall<TodoReply> GetAll(TodoFilterRequest request, CallOptions options)
        {
            return base.GetAll(request, options);
        }
    }
}