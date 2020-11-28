using Tickr.Models;

namespace Tickr
{
    public static class TodoExtensions
    {
        public static string SetCompleteMessage(this TodoModel todoModel)
        {
            if (todoModel == null)
                return "Todo not found";
            return todoModel.Complete ? "Todo Completed" : "Todo could not be completed";
        }
    }
}