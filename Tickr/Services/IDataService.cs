using System.Collections.Generic;
using System.Threading.Tasks;
using Tickr.Models;

namespace Tickr.Services
{
    public interface IDataService
    {
        Task<TodoModel> Add(TodoModel todoModel);
        Task<List<TodoModel>> GetAll(bool includeCompleted);

        Task<bool> Complete(string id);
    }
}