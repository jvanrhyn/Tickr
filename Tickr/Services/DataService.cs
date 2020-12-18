namespace Tickr.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Logging;
    using Models;
    using Repositories;
    using ResiliencePolicyHandlers;
    using Settings;

    public class DataService : IDataService
    {
        private readonly ILogger<DataService> _logger;
        private readonly TodoRepository _todoRepository;
        private readonly RetryPolicyHandler _retryPolicyHandler;
        private readonly ResilienceSettings _resilienceSettings;

        public DataService(ILogger<DataService> logger, TodoRepository todoRepository, RetryPolicyHandler retryPolicyHandler, ResilienceSettings resilienceSettings)
        {
            _logger = logger;
            _todoRepository = todoRepository;
            _retryPolicyHandler = retryPolicyHandler;
            _resilienceSettings = resilienceSettings;
        }

        [Authorize(Policy = "HasModifyScope")]
        public async Task<TodoModel> Add(TodoModel todoModel, CancellationToken cancellationToken = default)
        {
            return await _retryPolicyHandler.Retry(_resilienceSettings.RetryCount,
                () => _todoRepository.Create(todoModel, cancellationToken), cancellationToken);
        }

        [Authorize(Policy = "HasReadScope")]
        public async Task<List<TodoModel>> GetAll(bool includeCompleted, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Getting all todo model. Include completed is : {state}", includeCompleted);
            return await _retryPolicyHandler
                .Retry(_resilienceSettings.RetryCount,
                    () => (includeCompleted
                        ? _todoRepository.GetAll(cancellationToken)
                        : _todoRepository.GetFiltered(x => x.Complete == false, cancellationToken))
                    , cancellationToken);
        }

        [Authorize(Policy = "HasModifyScope")]
        public async Task<bool> Complete(string id, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Updating todo model for {key}", id);
            return await _retryPolicyHandler.Retry(_resilienceSettings.RetryCount, async () =>
            {
                var model = await _todoRepository.GetById(id, cancellationToken);
                if (model == null)
                {
                    throw new Exception($"TodoModel with key {id} not found");
                }    
                _ = await _todoRepository.Update(model, cancellationToken);
                return true;
            }, cancellationToken);
        }
    }
}