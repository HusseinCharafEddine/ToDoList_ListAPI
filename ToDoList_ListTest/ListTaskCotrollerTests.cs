//using NUnit.Framework;
//using Moq;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using ToDoList_ListAPI.Controllers;
//using ToDoList_ListAPI.Models.DTO;
//using ToDoList_ListAPI.Repository.IRepository;
//using ToDoList_ListAPI.Models;
//using ToDoList_ListAPI.Data;
//using Microsoft.AspNetCore.Mvc;
//using System.Net;
//using AutoMapper;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Threading.Tasks;
//using ToDoList_ListAPI;
//using Azure;

//namespace ToDoList_ListTest
//{
//    [TestFixture]
//    public class ListTaskControllerTests
//    {
//        private ListTaskController _listTaskController;
//        private Mock<IListTaskRepository> _listTaskRepoMock;
//        private DataFactory _DataFactory;
//        private ApplicationDbContext _dbContext;
//        private IMapper _mapper;

//        [SetUp]
//        public void Setup()
//        {
//            var serviceProvider = new ServiceCollection()
//                .AddEntityFrameworkInMemoryDatabase()
//                .BuildServiceProvider();

//            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
//                .UseInMemoryDatabase("TestDatabase")
//                .UseInternalServiceProvider(serviceProvider)
//                .Options;

//            _dbContext = new ApplicationDbContext(options);

//            _listTaskRepoMock = new Mock<IListTaskRepository>();
//            _DataFactory = new DataFactory(_dbContext);

//            // Initialize IMapper
//            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingConfig>()).CreateMapper();

//            _listTaskController = new ListTaskController(_listTaskContr.Object, _mapper);
//        }

//        [Test]
//        [TestCase(3, 1, HttpStatusCode.OK, true)]
//        public async Task GetListTasks_ValidRequest_ReturnsOkResult(int pageSize, int pageNumber, HttpStatusCode expectedStatusCode, bool expectedResult)
//        {
//            // Arrange
//            //var expectedList = _DataFactory.CreateTestListTasks();
            
//                _listTaskRepoMock
//                .Setup(repo => repo.GetAllAsync(
//                It.IsAny<Expression<Func<ListTask, bool>>>(),
//                It.IsAny<string>(),
//                It.IsAny<int>(),
//                It.IsAny<int>()
//            ))
//            .ReturnsAsync((Expression<Func<ListTask, bool>> filter, string includeProperties, int pageSize, int pageNumber) =>
//            {
//                // Mock behavior to return paginated result
//                var filteredList = _DataFactory.CreateTestListTasks().AsQueryable();

//                // Apply filter if provided
//                if (filter != null)
//                {
//                    filteredList = filteredList.Where(filter);
//                }

//                // Pagination
//                if (pageSize > 0)
//                {
//                    if (pageSize > 100)
//                    {
//                        pageSize = 100;
//                    }

//                    filteredList = filteredList.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
//                }

//                // Include related properties
//                if (!string.IsNullOrEmpty(includeProperties))
//                {
//                    foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
//                    {
//                        filteredList = filteredList.Include(includeProp);
//                    }
//                }

//                // Convert the filtered list to a ListTask collection
//                var result = filteredList.ToList();

//                // Return the result as if it came from the actual database
//                return result;
//            });
//            // Act
//            ActionResult<APIResponse> actionResult = await _listTaskController.GetListTasks(null, null, pageSize, pageNumber);

//            APIResponse result = (APIResponse)((ObjectResult)actionResult.Result).Value;
//            int resultSize = (result.Result as ICollection<ListTaskDTO>)?.Count ?? 0;
//            List<ListTaskDTO> testTasks = new List<ListTaskDTO>();
            

//            // Assert
//            Assert.NotNull(actionResult);
//            Assert.NotNull(result);
//            Assert.That(resultSize, Is.EqualTo(pageSize));
//            Assert.That(result.StatusCode, Is.EqualTo(expectedStatusCode));
//            Assert.That((result.IsSuccess), Is.EqualTo(expectedResult));
//        }
//        [Test]
//        [TestCase(-1, 1, HttpStatusCode.BadRequest, false)]
//        [TestCase(2, -1, HttpStatusCode.BadRequest, false)]
//        public async Task GetListTasks_ValidRequest_ReturnsBadRequest(int pageSize, int pageNumber, HttpStatusCode expectedStatusCode, bool expectedResult)
//        {
//            // Arrange
//            //var expectedList = _DataFactory.CreateTestListTasks();

//            _listTaskRepoMock
//            .Setup(repo => repo.GetAllAsync(
//            It.IsAny<Expression<Func<ListTask, bool>>>(),
//            It.IsAny<string>(),
//            It.IsAny<int>(),
//            It.IsAny<int>()
//        ))
//        .ReturnsAsync((Expression<Func<ListTask, bool>> filter, string includeProperties, int pageSize, int pageNumber) =>
//        {
//            // Mock behavior to return paginated result
//            var filteredList = _DataFactory.CreateTestListTasks().AsQueryable();

//            // Apply filter if provided
//            if (filter != null)
//            {
//                filteredList = filteredList.Where(filter);
//            }

//            // Pagination
//            if (pageSize > 0)
//            {
//                if (pageSize > 100)
//                {
//                    pageSize = 100;
//                }

//                filteredList = filteredList.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
//            }

//            // Include related properties
//            if (!string.IsNullOrEmpty(includeProperties))
//            {
//                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
//                {
//                    filteredList = filteredList.Include(includeProp);
//                }
//            }

//            // Convert the filtered list to a ListTask collection
//            var result = filteredList.ToList();

//            // Return the result as if it came from the actual database
//            return result;
//        });
//            // Act
//            ActionResult<APIResponse> actionResult = await _listTaskController.GetListTasks(null, null, pageSize, pageNumber);

//            APIResponse result = (APIResponse)(actionResult.Value);
//            int resultSize = (result.Result as ICollection<ListTaskDTO>)?.Count ?? 0;


//            // Assert
//            Assert.NotNull(actionResult);
//            Assert.Null(result.Result);
//            Assert.That(resultSize, Is.EqualTo(0));
//            Assert.That(result.StatusCode, Is.EqualTo(expectedStatusCode));
//            Assert.That((result.IsSuccess), Is.EqualTo(expectedResult));
//        }

//        public Mock<IListTaskRepository> Get_listTaskRepoMock()
//        {
//            return _listTaskRepoMock;
//        }

//        [Test]
//        [TestCase(1,"Title1","Task1", "sfasdf", false, HttpStatusCode.OK,true)]
//        public async Task GetListTask_ValidRequest_ReturnsOkResult(int id, string title, string category, string description, bool isCompleted, HttpStatusCode expectedStatusCode, bool expectedResult)
//        {
//            // Arrange
//            //var expectedList = _DataFactory.CreateTestListTasks();
//            _listTaskRepoMock
//            .Setup(repo => repo.GetAsync(
//            It.IsAny<Expression<Func<ListTask, bool>>>(),
//            It.IsAny<bool>(),
//            It.IsAny<string>()
//        ))
//        .ReturnsAsync((Expression<Func<ListTask, bool>> filter, bool tracked, string includeProperties) =>
//        {
//            // Mock behavior to return paginated result
//            var filteredList = _DataFactory.CreateTestListTasks().AsQueryable();
//            tracked = false;
//            if (!tracked)
//            {
//                filteredList = filteredList.AsNoTracking();
//            }
//            if (filter != null)
//            {
//                filteredList = filteredList.Where(filter);
//            }

//            if (includeProperties != null)
//            {
//                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
//                {
//                    filteredList = filteredList.Include(includeProp);
//                }
//            }

//            ListTask task = filteredList.FirstOrDefault();
//            return task;
//        });
//            var listTask = _DataFactory.CreateTestListTask(id, title, category, description, DateTime.MinValue, isCompleted);

//            // Act
//            ActionResult<APIResponse> actionResult = await _listTaskController.GetListTask(id);

//            APIResponse result = (APIResponse)((ObjectResult)actionResult.Result).Value;
//            int resultSize = (result.Result as ICollection<ListTaskDTO>)?.Count ?? 0;
//            List<ListTaskDTO> testTasks = new List<ListTaskDTO>();


//            // Assert
//            Assert.NotNull(actionResult);
//            Assert.NotNull(result);
//            Assert.That(resultSize, Is.EqualTo(1));
//            Assert.That(result.StatusCode, Is.EqualTo(expectedStatusCode));
//            Assert.That((result.IsSuccess), Is.EqualTo(expectedResult));
//        }
        

//    }
//}
