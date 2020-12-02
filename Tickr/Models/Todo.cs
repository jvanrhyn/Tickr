namespace Tickr.Models
{
    using System;
    using Google.Protobuf.WellKnownTypes;

    public class TodoModel
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Id { get; set; }
        public string Description { get; init; }
        public DateTime Created { get; init; }
        public bool Complete { get; set; }

        public TodoReply ToResponse()
        {
            return new()
            {
                Complete = Complete,
                Created = Timestamp.FromDateTime(Created),
                Description = Description,
                Id = Id
            };
        }
    }
}