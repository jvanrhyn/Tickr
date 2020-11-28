using System;
using Google.Protobuf.WellKnownTypes;

namespace Tickr.Models
{
    public class TodoModel
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
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