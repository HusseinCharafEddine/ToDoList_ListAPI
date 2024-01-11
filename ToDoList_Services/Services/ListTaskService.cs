using AutoMapper;
using Azure;
using System.Linq.Expressions;
using System.Net;
using ToDoList_Repository.Repository.IRepository;
using ToDoList_Utility.Models;
using ToDoList_Utility.Models.DTO;
using ToDoList_Services.Services.IServices;
using ToDoList_Utility.Validators;
using FluentValidation;
using ToDoList_ListAPI.Validators;
using ToDoList_Utility.Models.Exceptions;

namespace ToDoList_Services.Services
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

            Func<ListTask, bool> predicate = u =>
             (string.IsNullOrEmpty(search) || u.Title.ToLower().Contains(search)) &&
             (string.IsNullOrEmpty(category) || u.Category.ToLower().Contains(category));
             
            listTaskList = listTaskList.Where(predicate);
            return _mapper.Map<List<ListTaskDTO>>(listTaskList); ;
        }
        public async Task<ListTaskDTO> GetAsync(int id)
        {
            if (id <= 0)
            {
                throw new BadRequestException(1);
            }
            var listTask = await _listTaskRepo.GetAsync(u => u.Id == id) ?? throw new NotFoundException(5);
            return _mapper.Map<ListTaskDTO>(listTask);
        }
        public async Task<ListTaskDTO> CreateAsync(ListTaskCreateDTO createDTO)
        {
            if (createDTO == null)
            {
                throw new BadRequestException(2);
            }
           
            if (await _listTaskRepo.GetAsync(u => u.Title.ToLower() == createDTO.Title.ToLower()) != null)
            {
                throw new BadRequestException(3);

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
                throw new BadRequestException(1);
            }
            var listTask = await _listTaskRepo.GetAsync(u => u.Id == id);
            if ( listTask == null)
            {
                throw new NotFoundException(5);
            }
            await _listTaskRepo.RemoveAsync(listTask);

        }
    public async Task UpdateAsync(int id, ListTaskUpdateDTO updateDTO)
        {
            if (id <= 0)
            {
                throw new BadRequestException(1);
            }
            
            if (updateDTO == null || id != updateDTO.Id)
            {
                throw new BadRequestException(2);
            }
            if (id != updateDTO.Id)
            {
                throw new BadRequestException(4);
            }
            
            ListTask model = _mapper.Map<ListTask>(updateDTO);
            await _listTaskRepo.UpdateAsync(model);
        }              
    }
}
