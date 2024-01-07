using AutoMapper;
using Microsoft.Extensions.Options;
using ToDoList_Utility.Models;
using ToDoList_Utility.Models.DTO;
namespace ToDoList_Services
{
    public class MappingConfig : Profile

    {
        public MappingConfig()         {
            CreateMap<ListTask, ListTaskDTO>().ReverseMap();
            CreateMap<ListTask, ListTaskCreateDTO>().ReverseMap();
            CreateMap<ListTask, ListTaskUpdateDTO>().ReverseMap();

        }
    }
}
