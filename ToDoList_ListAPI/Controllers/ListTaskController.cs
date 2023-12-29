using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using ToDoList_ListAPI.Models;
using ToDoList_ListAPI.Models.DTO;
using ToDoList_ListAPI.Repository.IRepository;

namespace ToDoList_ListAPI.Controllers
{
    [Route("api/tasks")]
    [ApiController]
    public class ListTaskController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IMapper _mapper;
        private readonly IListTaskRepository _dbListTask;

        public ListTaskController(IListTaskRepository dbListTask, IMapper mapper)
        {
            _dbListTask = dbListTask;
            _mapper = mapper;
            _response = new();
        }

        
        [HttpGet]
        [AllowAnonymous]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetListTasks([FromQuery(Name = "filterCategory")] string? category, [FromQuery] string? search
            , int pageSize = 0, int pageNumber = 1)
        {
            try
            {
                IEnumerable<ListTask> listTaskList;

                if (pageSize < 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    string ex = $"Page size can not be negative. {pageSize}";
                    _response.ErrorMessages = new List<string>() { ex};
                    throw new ArgumentException(ex);
                }

                if(pageNumber < 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    string ex = $"Page size can not be negative. {pageNumber}";
                    _response.ErrorMessages = new List<string>() { ex };
                    throw new ArgumentException(ex);
                }
                
                listTaskList = await _dbListTask.GetAllAsync(pageSize: pageSize, pageNumber: pageNumber);

                if (!string.IsNullOrEmpty(search))
                {
                    listTaskList = listTaskList.Where(u => u.Title.ToLower().Contains(search));
                }
                if (!string.IsNullOrEmpty(category))
                {
                    listTaskList = listTaskList.Where(u => u.Category.ToLower().Contains(category));
                }
                Pagination pagination = new() { PageNumber = pageNumber, PageSize = pageSize };
                _response.Result = _mapper.Map<List<ListTaskDTO>>(listTaskList);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                if (_response.StatusCode == null)
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpGet("{id:int}", Name = "GetListTask")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetListTask(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                var listTask = await _dbListTask.GetAsync(u => u.Id == id);
                if (listTask == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
                _response.Result = _mapper.Map<ListTaskDTO>(listTask);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<APIResponse>> CreateListTask([FromBody] ListTaskCreateDTO createDTO)
        {
            try
            {
                if (await _dbListTask.GetAsync(u => u.Title.ToLower() == createDTO.Title.ToLower()) != null)

                {
                    ModelState.AddModelError("ErrorMessages", "Task Already Exists");
                    return BadRequest(ModelState);
                }
                if (createDTO == null)
                {
                    return BadRequest(createDTO);
                }
                ListTask listTask = _mapper.Map<ListTask>(createDTO);
                await _dbListTask.CreateAsync(listTask);
                _response.Result = _mapper.Map<ListTaskDTO>(listTask);
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetListTask", new { id = listTask.Id }, _response);
            } catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id:int}", Name = "DeleteListTaskApi")]
        [Authorize(Roles = "admin")]

        public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
        {
            try
            {
                if (id == 0) {
                    return BadRequest();

                }
                var listTask = await _dbListTask.GetAsync(u => u.Id == id);
                if (listTask == null)
                {
                    return NotFound();
                }
                await _dbListTask.RemoveAsync(listTask);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);

            } catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id:int}", Name = "UpdateVillaApi")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody] ListTaskUpdateDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || id != updateDTO.Id)
                {
                    return BadRequest();
                }
                ListTask model = _mapper.Map<ListTask>(updateDTO);
                await _dbListTask.UpdateAsync(model);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
    }
}
