namespace Tickr.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Logging;
    using Raven.Client.Documents;
    using Raven.Client.Documents.Linq;
    using Models;
    using Polly;
    using ResiliencePolicyHandlers;
    using Settings;

    public class DataService : IDataService
    {
        private readonly ILogger<DataService> _logger;
        private readonly DataSource _dataSource;
        private readonly RetryPolicyHandler _retryPolicyHandler;
        private readonly ResilienceSettings _resilienceSettings;

        public DataService(ILogger<DataService> logger, DataSource dataSource, RetryPolicyHandler retryPolicyHandler, ResilienceSettings resilienceSettings)
        {
            _logger = logger;
            _dataSource = dataSource;
            _retryPolicyHandler = retryPolicyHandler;
            _resilienceSettings = resilienceSettings;
        }
        
        [Authorize(Policy = "HasModifyScope")]
        public async Task<TodoModel> Add(TodoModel todoModel, CancellationToken cancellationToken = default)
        {
            return await _retryPolicyHandler.Retry(_resilienceSettings.RetryCount, () => AddImpl(todoModel), cancellationToken);
        }
        
        [Authorize(Policy = "HasReadScope")]
        public async Task<List<TodoModel>> GetAll(bool includeCompleted, CancellationToken cancellationToken = default)
        {
            return await _retryPolicyHandler
               .Retry(_resilienceSettings.RetryCount, () => GetAllImpl(includeCompleted), cancellationToken);
        }

        [Authorize(Policy = "HasModifyScope")]
        public async Task<bool> Complete(string id, CancellationToken cancellationToken = default)
        {
            return await _retryPolicyHandler.Retry(_resilienceSettings.RetryCount, () => CompleteImpl(id), cancellationToken);
        }

        private async Task<TodoModel> AddImpl(TodoModel todoModel)
        {
            _logger.LogDebug("Adding new todoModel {todo}", todoModel);
            var store = _dataSource.Store;

            using var session = store.OpenAsyncSession();
            await session.StoreAsync(todoModel);
            await session.SaveChangesAsync();

            return todoModel;
        }

        private async Task<List<TodoModel>> GetAllImpl(bool includeCompleted)
        {
            var store = _dataSource.Store;
            using var session = store.OpenAsyncSession();

            var models = session.Query<TodoModel>();
            if (!includeCompleted)
                models = Queryable.Where(models, x => x.Complete == false) as IRavenQueryable<TodoModel>;

            return await models.ToListAsync();
        }

        private async Task<bool> CompleteImpl(string id)
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