namespace Tickr.Models
{
    using System;

    public class TodoModel : BaseModel
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global

        public string Description { get; init; }
        public DateTime Created { get; init; }
        public bool Complete { get; set; }

        public override string ToString()
        {
            return $"Todo | Id:{Id} | Description : {Description}| Complete: {Complete}";
        }
    }
}