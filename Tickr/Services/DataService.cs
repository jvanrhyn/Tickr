namespace Tickr.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Raven.Client.Documents;
    using Raven.Client.Documents.Linq;
    using Tickr.Models;

    public class DataService : IDataService
    {
        private readonly ILogger<DataService> _logger;
        private readonly DataSource _dataSource;

        public DataService(ILogger<DataService> logger, DataSource dataSource)
        {
            _logger = logger;
            _dataSource = dataSource;
        }

        public async Task<TodoModel> Add(TodoModel todoModel)
        {
            _logger.LogDebug("Adding new todoModel {todo}", todoModel);
            var store = _dataSource.Store;

            using var session = store.OpenAsyncSession();
            await session.StoreAsync(todoModel);
            await session.SaveChangesAsync();

            return todoModel;
        }

        public async Task<List<TodoModel>> GetAll(bool includeCompleted)
        {
            var store = _dataSource.Store;
            using var session = store.OpenAsyncSession();

            var models = session.Query<TodoModel>();
            if (!includeCompleted)
                models = Queryable.Where(models, x => x.Complete == false) as IRavenQueryable<TodoModel>;

            return await models.ToListAsync();
        }

        public async Task<bool> Complete(string id)
        {
            var store = _dataSource.Store;

            using var session = store.OpenAsyncSession();
            var todoModel = await session.LoadAsync<TodoModel>(id);

            if (todoModel == null)
            {
                _logger.LogWarning("Todo '{id}' could not be found to complete.", id);
                throw new TodoNotFoundException(id);
            }

            _logger.LogInformation($"Completing todo {todoModel}", todoModel);
            todoModel.Complete = true;

            await session.StoreAsync(todoModel);
            await session.SaveChangesAsync();

            return true;
        }
    }
}