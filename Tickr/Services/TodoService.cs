using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Tickr.Models;

namespace Tickr
{
    public class TodoService : Todo.TodoBase
    {
        private readonly IDataService _dataService;
        private readonly ILogger<TodoService> _logger;

        public TodoService(ILogger<TodoService> logger, IDataService dataService)
        {
            _logger = logger;
            _dataService = dataService;
        }

        public override Task<TodoReply> Add(TodoRequest request, ServerCallContext context)
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

            _dataService.Add(todoModel);

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
            return Task.FromResult(response);
        }

        public override async Task GetAll(TodoFilterRequest request, IServerStreamWriter<TodoReply> responseStream,
            ServerCallContext context)
        {
            foreach (var todoModel in _dataService.GetAll(request.IncludeCompleted))
                await responseStream.WriteAsync(todoModel.ToResponse());
        }

        public override Task<CompleteReply> Complete(CompleteRequest request, ServerCallContext context)
        {
            CompleteReply completeReply = new() {Id = request.Id};

            try
            {
                var success = _dataService.Complete(request.Id);
                completeReply.Status = success ? "Completed" : "Not completed";
                return Task.FromResult(completeReply);
            }
            catch (TodoNotFoundException nfe)
            {
                completeReply.Status = nfe.ToString();
                return Task.FromResult(completeReply);
            }
        }
    }
}