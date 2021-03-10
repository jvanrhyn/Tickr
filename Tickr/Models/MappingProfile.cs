namespace Tickr.Models
{
    using AutoMapper;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TodoModel, TodoModel>();
        }
    }
}
