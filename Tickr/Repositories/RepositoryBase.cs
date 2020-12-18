namespace Tickr.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.Extensions.Logging;
    using Models;
    using Raven.Client.Documents;
    using Raven.Client.Documents.Linq;
    using Services;

    public abstract class RepositoryBase<TModel> where TModel : BaseModel
    {
        private readonly ILogger _logger;
        private readonly DataSource _dataSource;
        private readonly IMapper _mapper;

        protected RepositoryBase(ILogger logger, DataSource dataSource, IMapper mapper )
        {
            _logger = logger;
            _dataSource = dataSource;
            _mapper = mapper;
        }

        public virtual async Task<TModel> GetById(string key, CancellationToken cancellationToken = default)
        {
            using var scope = _logger.BeginScope(this);
            _logger.LogDebug("Query model {model} by key : {key}", typeof(TModel).Name, key);

            var store = _dataSource.Store;
            using var session = store.OpenAsyncSession();

            var model = await session.LoadAsync<TModel>(key, cancellationToken);

            scope.Dispose();
            return model;
        }

        public virtual async Task<List<TModel>> GetFiltered(Expression<Func<TModel, bool>> filter, CancellationToken cancellationToken = default)
        {
            var store = _dataSource.Store;
            using var session = store.OpenAsyncSession();

            var models = session.Query<TModel>();
                models = Queryable.Where(models, filter) as IRavenQueryable<TModel>;

            return await models.ToListAsync(token: cancellationToken);
        }

        public virtual async Task<List<TModel>> GetAll(CancellationToken cancellationToken = default)
        {
            var store = _dataSource.Store;
            using var session = store.OpenAsyncSession();

            var models = session.Query<TModel>();
            models = Queryable.Where(models, x =>!string.IsNullOrEmpty(x.Id) ) as IRavenQueryable<TModel>;

            return await models.ToListAsync(token: cancellationToken);
        }

        public virtual async Task<TModel> Update(TModel model, CancellationToken cancellationToken = default)
        {
            var key = model.Id;
            using var scope = _logger.BeginScope(this);
            _logger.LogDebug("Query model {model} by key '{key}' for update", typeof(TModel).Name, key);

            var store = _dataSource.Store;
            using var session = store.OpenAsyncSession();

            var originalModel = await session.LoadAsync<TModel>(key, cancellationToken);

            if (originalModel == null)
            {
                _logger.LogWarning("{model} with key {key} not found", typeof(TModel).Name, key);
                throw new Exception($"{typeof(TModel).Name} with key {key} not found");
            }
            
            await session.StoreAsync(model, cancellationToken);
            await session.SaveChangesAsync(cancellationToken);
            
            scope.Dispose();
            return model;
        }

        public virtual async Task<TModel> Create(TModel model, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Adding new {model} {name}", typeof(TModel).Name, model);
            var store = _dataSource.Store;

            using var session = store.OpenAsyncSession();
            await session.StoreAsync(model, cancellationToken);
            await session.SaveChangesAsync(cancellationToken);

            return model;
        }
    }
}
