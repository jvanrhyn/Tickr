namespace Tickr.Services
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Models;

    public interface IDataService
    {
        Task<TodoModel> Add(TodoModel todoModel, CancellationToken cancellationToken = default);
        Task<List<TodoModel>> GetAll(bool includeCompleted, CancellationToken cancellationToken = default);

        Task<bool> Complete(string id, CancellationToken cancellationToken = default);
    }
}