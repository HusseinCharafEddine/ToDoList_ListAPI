using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Net;
using ToDoList_ListAPI.Models;
using ToDoList_ListAPI.Models.DTO;
using ToDoList_ListAPI.Repository.IRepository;
using ToDoList_ListAPI.Services.IServices;

namespace ToDoList_ListAPI.Services
{
    public class ListTaskService : Service<ListTask>, IListTaskService
    {
        private readonly IListTaskRepository _listTaskRepo;
        private readonly IMapper _mapper;

        public ListTaskService(IListTaskRepository listTaskRepo, IMapper mapper)
            : base(listTaskRepo)
        {
            _listTaskRepo = listTaskRepo;
            _mapper = mapper;
        }
        public async Task<List<ListTaskDTO>> GetAllAsync(string? category, string? search, int pageSize = 0, int pageNumber = 1)
        {
            IEnumerable<ListTask> listTaskList = await _listTaskRepo.GetAllAsync(pageSize: pageSize, pageNumber: pageNumber);
            if (pageSize < 0)
            {
                string ex = $"Page size can not be negative. {pageSize}";
                throw new BadRequestException(ex);
            }

            if (pageNumber < 0)
            {
                string ex = $"Page number can not be negative. {pageNumber}";
                throw new BadRequestException(ex);
            }
            if (!string.IsNullOrEmpty(search))
            {
                listTaskList = (List<ListTask>)listTaskList.Where(u => u.Title.ToLower().Contains(search));
            }
            if (!string.IsNullOrEmpty(category))
            {
                listTaskList = (List<ListTask>)listTaskList.Where(u => u.Category.ToLower().Contains(category));
            }
            return _mapper.Map<List<ListTaskDTO>>(listTaskList); ;
        }
        public async Task<ListTaskDTO> GetAsync(int id)
        {
            if (id <= 0)
            {
                string ex = "Id can not be nonpositive";
                throw new BadRequestException(ex);
            }
            var listTask = await _listTaskRepo.GetAsync(u => u.Id == id);
            if (listTask == null)
            {
                string ex = "Task has not been found";
                throw new NotFoundException(ex);
            }
            return _mapper.Map<ListTaskDTO>(listTask);
        }
        public async Task<ListTaskDTO> CreateAsync(ListTaskCreateDTO createDTO)
        {
            if (createDTO == null || createDTO.Title == null)
            {
                throw new BadRequestException("Some fields are empty please fill required fields");
            }
            if (await _listTaskRepo.GetAsync(u => u.Title.ToLower() == createDTO.Title.ToLower()) != null)
            {
                throw new BadRequestException("Task with the title '" + createDTO.Title + "' already exists");

            }
            
            ListTask listTask = _mapper.Map<ListTask>(createDTO);
            await _listTaskRepo.CreateAsync(listTask);
            ListTaskDTO listTaskDTO = _mapper.Map<ListTaskDTO>(listTask);
            return listTaskDTO;
        }
        
        public async Task RemoveAsync(int id )
        {
            if (id <= 0)
            {
                string ex = "Id can not be nonpositive";
                throw new BadRequestException(ex);
            }
            var listTask = await _listTaskRepo.GetAsync(u => u.Id == id);
            if ( listTask == null)
            {
                string ex = "Task has not been found";
                throw new NotFoundException(ex);
            }
            await _listTaskRepo.RemoveAsync(listTask);

        }
    public async Task UpdateAsync(int id, ListTaskUpdateDTO updateDTO)
        {
            if (id <= 0)
            {
                string ex = "Id can not be nonpositive";
                throw new BadRequestException(ex);
            }
            
            if (updateDTO == null || id != updateDTO.Id)
            {
                string ex = "Task is null, or id toesnt match that of the task";
                throw new BadRequestException(ex);
            }
            if (updateDTO.Title == null)
            {
                string ex = "Title can not be empty";
                throw new BadRequestException(ex);
            }
            ListTask model = _mapper.Map<ListTask>(updateDTO);
            await _listTaskRepo.UpdateAsync(model);
        }

      


    }
}
