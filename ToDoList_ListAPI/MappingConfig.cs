using AutoMapper;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using ToDoList_ListAPI.Models;
using ToDoList_ListAPI.Models.DTO;
namespace ToDoList_ListAPI
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
