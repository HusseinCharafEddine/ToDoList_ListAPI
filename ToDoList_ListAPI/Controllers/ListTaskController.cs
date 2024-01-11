using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using ToDoList_Utility.Models;
using ToDoList_Utility.Models.DTO;
using ToDoList_Services.Services.IServices;

namespace ToDoList_ListAPI.Controllers
{
    [Route("api/tasks")]
    [ApiController]
    public class ListTaskController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IMapper _mapper;
        private readonly IListTaskService _listTaskService;

        public ListTaskController(IListTaskService listTaskService, IMapper mapper)
        {
            _listTaskService= listTaskService;
            _mapper = mapper;
            _response = new();
        }

        
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
            public async Task<ActionResult<APIResponse>> GetListTasks([FromQuery(Name = "filterCategory")] string? category, [FromQuery] string? search
                , int pageSize = 0, int pageNumber = 1)
        {
            //try
            //{
                 var listTasks= await _listTaskService.GetAllAsync(category, search, pageSize, pageNumber);
                _response.Result = _mapper.Map<List<ListTaskDTO>>(listTasks);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);

            //}
            //catch (BadRequestException ex)
            //{
            //    _response.IsSuccess = false;
            //    _response.StatusCode = HttpStatusCode.BadRequest;
            //    _response.ErrorMessages = new List<string> () { ex.ToString()} ;
            //    return BadRequest(_response);

            //}
            //catch (Exception ex)
            //{
            //    _response.IsSuccess = false;
            //    if (_response.StatusCode == null)
            //    _response.StatusCode = HttpStatusCode.InternalServerError;
            //    _response.ErrorMessages = new List<string>() { ex.ToString() };
            //}
            //return _response;
        }

        [HttpGet("{id:int}", Name = "GetListTask")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetListTask(int id)
        {
            //try
            //{

                var listTask = await _listTaskService.GetAsync(id);

                _response.Result = _mapper.Map<ListTaskDTO>(listTask);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            //}
            //catch (BadRequestException ex)
            //{
            //    _response.IsSuccess = false;
            //    _response.StatusCode = HttpStatusCode.BadRequest;
            //    _response.ErrorMessages = new List<string>() { ex.ToString() };

            //}
            //catch (Exception ex)
            //{
            //    _response.IsSuccess = false;
            //    _response.StatusCode = HttpStatusCode.InternalServerError;
            //    _response.ErrorMessages = new List<string>() { ex.ToString() };
            //}
            //return _response;
        }

        [HttpPost]
        //[Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<APIResponse>> CreateListTask([FromBody] ListTaskCreateDTO createDTO)
        {
            //try
            //{
                
                ListTaskDTO listTask = await _listTaskService.CreateAsync(createDTO);
                _response.Result = _mapper.Map<ListTaskDTO>(listTask);
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetListTask", new { id = listTask.Id }, _response);
            //}
            //catch (BadRequestException ex)
            //{
            //    _response.IsSuccess = false;
            //    _response.StatusCode = HttpStatusCode.BadRequest;
            //    _response.ErrorMessages = new List<string>() { ex.ToString() };
            //}
            //catch (Exception ex)
            //{
            //    _response.IsSuccess = false;
            //    _response.StatusCode = HttpStatusCode.InternalServerError;
            //    _response.ErrorMessages = new List<string>() { ex.ToString() };
            //}
            //return _response;
        }
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id:int}", Name = "DeleteListTaskApi")]
        //[Authorize(Roles = "admin")]

        public async Task<ActionResult<APIResponse>> DeleteListTask(int id)
        {
            //try
            //{
                
                await _listTaskService.RemoveAsync(id);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);

            //}
            //catch (BadRequestException ex)
            //{
            //    _response.IsSuccess = false;
            //    _response.StatusCode = HttpStatusCode.BadRequest;
            //    _response.ErrorMessages = new List<string>() { ex.ToString() };
            //}
            //catch (NotFoundException ex)
            //{
            //    _response.IsSuccess = true;
            //    _response.StatusCode = HttpStatusCode.NotFound;
            //    _response.ErrorMessages = new List<string>() { ex.ToString() };

            //}
            //catch (Exception ex)
            //{
            //    _response.IsSuccess = false;
            //    _response.StatusCode = HttpStatusCode.InternalServerError;
            //    _response.ErrorMessages = new List<string>() { ex.ToString() };
            //}
            //return _response;
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id:int}", Name = "UpdateListTaskApi")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateListTask(int id, [FromBody] ListTaskUpdateDTO updateDTO)
        {
            //try
            //{
                await _listTaskService.UpdateAsync(id, updateDTO);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            //}
            //catch (Exception ex)
            //{
                //_response.IsSuccess = false;
            //    _response.StatusCode = HttpStatusCode.InternalServerError;
            //    _response.ErrorMessages = new List<string>() { ex.ToString() };
            //}
            //return _response;
        }
        HttpPost]
        //[Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<APIResponse>> AddLineSaveToPDF([FromBody] ListTaskCreateDTO createDTO)
        {
            //try
            //{

            ListTaskDTO listTask = await _listTaskService.CreateAsyncLineToPDF(createDTO);
            _response.Result = _mapper.Map<ListTaskDTO>(listTask);
            _response.StatusCode = HttpStatusCode.Created;
            return CreatedAtRoute("GetListTask", new { id = listTask.Id }, _response);
            //}
            //catch (BadRequestException ex)
            //{
            //    _response.IsSuccess = false;
            //    _response.StatusCode = HttpStatusCode.BadRequest;
            //    _response.ErrorMessages = new List<string>() { ex.ToString() };
            //}
            //catch (Exception ex)
            //{
            //    _response.IsSuccess = false;
            //    _response.StatusCode = HttpStatusCode.InternalServerError;
            //    _response.ErrorMessages = new List<string>() { ex.ToString() };
            //}
            //return _response;
        }
    }
}
