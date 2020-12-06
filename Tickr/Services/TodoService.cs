namespace Tickr.Services
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Google.Protobuf.WellKnownTypes;
    using Grpc.Core;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Logging;
    using Models;

    public class TodoService : Todo.TodoBase
    {
        private readonly IDataService _dataService;
        private readonly ILogger<TodoService> _logger;
        
        public TodoService(ILogger<TodoService> logger, IDataService dataService)
        {
            _logger = logger;
            _dataService = dataService;
        }

        [Authorize(Policy = "HasModifyScope")]
        public override async Task<TodoReply> Add(TodoRequest request, ServerCallContext context)
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();
            _logger.LogInformation("Creating new Todo {description}", request.Description);
            TodoModel todoModel = new()
            {
                Complete = request.Complete,
                Created = DateTime.Now.ToUniversalTime(),
                Description = request.Description
            };

            await _dataService.Add(todoModel, context.CancellationToken);

            TodoReply response = new()
            {
                Id = todoModel.Id,
                Complete = todoModel.Complete,
                Created = Timestamp.FromDateTime(todoModel.Created),
                Description = todoModel.Description
            };

            _logger.LogInformation("Completed creation for {id} in {duration}ms", todoModel.Id,
                stopwatch.ElapsedMilliseconds);
            stopwatch.Stop();
            return response;
        }

        [Authorize(Policy = "HasReadScope")]
        public override async Task GetAll(TodoFilterRequest request, IServerStreamWriter<TodoReply> responseStream,
            ServerCallContext context)
        {
            foreach (var todoModel in await _dataService.GetAll(request.IncludeCompleted, context.CancellationToken))
                await responseStream.WriteAsync(todoModel.ToResponse());
        }

        [Authorize(Policy = "HasModifyScope")]
        public override async Task<CompleteReply> Complete(CompleteRequest request, ServerCallContext context)
        {
            CompleteReply completeReply = new() {Id = request.Id};

            try
            {
                var success = await _dataService.Complete(request.Id, context.CancellationToken);
                completeReply.Status = success ? "Completed" : "Not completed";
                return completeReply;
            }
            catch (TodoNotFoundException nfe)
            {
                completeReply.Status = nfe.ToString();
                return completeReply;
            }
        }
    }
}
