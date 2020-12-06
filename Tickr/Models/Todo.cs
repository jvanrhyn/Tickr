namespace Tickr.Models
{
    using System;

    public class TodoModel
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Id { get; set; }
        public string Description { get; init; }
        public DateTime Created { get; init; }
        public bool Complete { get; set; }

        public override string ToString()
        {
            return $"Todo | Id:{Id} | Description : {Description}| Complete: {Complete}";
        }
    }
}