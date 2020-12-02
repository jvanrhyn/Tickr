namespace Tickr.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    public interface IDataService
    {
        Task<TodoModel> Add(TodoModel todoModel);
        Task<List<TodoModel>> GetAll(bool includeCompleted);

        Task<bool> Complete(string id);
    }
}