namespace Tickr.Models
{
    using Google.Protobuf.WellKnownTypes;

    public static class ModelExtensions
    {
        public static TodoReply ToReply(this TodoModel model)
        {
            return new()
            {
                Complete = model.Complete,
                Created = Timestamp.FromDateTime(model.Created),
                Description = model.Description,
                Id = model.Id
            };
        }
        
    }
}