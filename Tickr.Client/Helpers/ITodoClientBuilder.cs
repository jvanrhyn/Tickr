namespace Tickr.Client.Helpers
{
    using System.Threading.Tasks;

    public interface ITodoClientBuilder
    {
        Task<Todo.TodoClient> Create();
    }
}