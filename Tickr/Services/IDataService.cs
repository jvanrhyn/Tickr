using System.Collections.Generic;
using Tickr.Models;

namespace Tickr
{
    public interface IDataService
    {
        TodoModel Add(TodoModel todoModel);
        List<TodoModel> GetAll(bool includeCompleted);

        bool Complete(string id);
    }
}