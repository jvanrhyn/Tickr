namespace Tickr.Repositories
{
    using AutoMapper;
    using Microsoft.Extensions.Logging;
    using Models;
    using Services;

    public sealed class TodoRepository : RepositoryBase<TodoModel>
    {
        public TodoRepository(ILogger<TodoRepository> logger, DataSource dataSource, IMapper mapper) : base(logger, dataSource, mapper)
        {
        }
    }
}