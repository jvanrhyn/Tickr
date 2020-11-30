using System;

namespace Tickr.Services
{
    public class TodoNotFoundException : Exception
    {
        public TodoNotFoundException(string id) : base(id)
        {
        }

        public override string ToString()
        {
            return "Not found.";
        }
    }
}