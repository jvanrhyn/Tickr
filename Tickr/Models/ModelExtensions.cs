namespace Tickr.Models
{
    using Google.Protobuf.WellKnownTypes;
    using Talista.Utilities.Encoding;

    public static class ModelExtensions
    {
        public static TodoReply ToReply(this TodoModel model, IIdentifierMasking identifierMasking)
        {
            return new()
            {
                Complete = model.Complete,
                Created = Timestamp.FromDateTime(model.Created),
                Description = model.Description,
                Id = identifierMasking.HideIdentifier(model.Id)
            };
        }
        
    }
}