using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Linq;
using Tickr.Models;

namespace Tickr
{
    public class DataService : IDataService
    {
        private readonly ILogger<DataService> _logger;
        private readonly DataSource _dataSource;

        public DataService(ILogger<DataService> logger, DataSource dataSource)
        {
            _logger = logger;
            _dataSource = dataSource;
        }

        public TodoModel Add(TodoModel todoModel)
        {
            _logger.LogDebug("Adding new todoModel {todo}", todoModel);
            var store = _dataSource.Store;

            using var session = store.OpenSession();
            session.Store(todoModel);
            session.SaveChanges();

            return todoModel;
        }

        public List<TodoModel> GetAll(bool includeCompleted)
        {
            var store = _dataSource.Store;
            using var session = store.OpenSession();

            var models = session.Query<TodoModel>();
            if (!includeCompleted)
                models = Queryable.Where(models, x => x.Complete == false) as IRavenQueryable<TodoModel>;

            return models.ToList();
        }

        public bool Complete(string id)
        {
            var store = _dataSource.Store;

            using var session = store.OpenSession();
            var todoModel = session.Load<TodoModel>(id);

            if (todoModel == null)
            {
                _logger.LogWarning("Todo '{id}' could not be found to complete.", id);
                throw new TodoNotFoundException(id);
            }

            _logger.LogInformation($"Completing todo {todoModel}", todoModel);
            todoModel.Complete = true;

            session.Store(todoModel);
            session.SaveChanges();

            return true;
        }
    }
}