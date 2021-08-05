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
    using Talista.Utilities.Encoding;

    public class TodoService : Todo.TodoBase
    {
        private readonly IDataService _dataService;
        private readonly IIdentifierMasking _identifierMasking;
        private readonly ILogger<TodoService> _logger;
        
        public TodoService(ILogger<TodoService> logger, IDataService dataService, IIdentifierMasking identifierMasking)
        {
            _logger = logger;
            _dataService = dataService;
            _identifierMasking = identifierMasking;
        }

        [Authorize(Policy = "HasModifyScope")]
        public override async Task<TodoReply> Add(TodoRequest request, ServerCallContext context)
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();
            _logger.LogInformation("Creating new Todo {description}", request.Description);
            
            var todoModel = BuildTodoRequestModel(request);
            await _dataService.Add(todoModel, context.CancellationToken);
            var response = BuildTodoReply(todoModel);

            _logger.LogInformation("Completed creation for {id} in {duration}ms", todoModel.Id,
                stopwatch.ElapsedMilliseconds);
            stopwatch.Stop();
            return response;
        }


        [Authorize(Policy = "HasReadScope")]
        public override async Task GetAll(TodoFilterRequest request, IServerStreamWriter<TodoReply> responseStream,
            ServerCallContext context)
        {
            foreach (var todoModel in await _dataService.GetAll(request.IncludeCompleted))
                await responseStream.WriteAsync(todoModel.ToReply(_identifierMasking));
        }

        [Authorize(Policy = "HasModifyScope")]
        public override async Task<CompleteReply> Complete(CompleteRequest request, ServerCallContext context)
        {
            CompleteReply completeReply = new() {Id = request.Id};

            try
            {
                var success = await _dataService.Complete(_identifierMasking.RevealIdentifier(request.Id),
                    context.CancellationToken);
                completeReply.Status = success ? "Completed" : "Not completed";
                return completeReply;
            }
            catch (TodoNotFoundException nfe)
            {
                completeReply.Status = nfe.ToString();
                return completeReply;
            }
        }

        internal TodoReply BuildTodoReply(TodoModel todoModel)
        {
            TodoReply response = new()
            {
                Id = _identifierMasking.HideIdentifier(todoModel.Id),
                Complete = todoModel.Complete,
                Created = Timestamp.FromDateTime(todoModel.Created),
                Description = todoModel.Description
            };
            return response;
        }

        internal static TodoModel BuildTodoRequestModel(TodoRequest request)
        {
            TodoModel todoModel = new()
            {
                Complete = request.Complete,
                Created = DateTime.Now.ToUniversalTime(),
                Description = request.Description
            };
            return todoModel;
        }
    }
}
