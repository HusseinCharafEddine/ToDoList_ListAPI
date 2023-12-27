using NUnit.Framework;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ToDoList_ListAPI.Controllers;
using ToDoList_ListAPI.Models.DTO;
using ToDoList_ListAPI.Repository.IRepository;
using ToDoList_ListAPI.Models;
using ToDoList_ListAPI.Data;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ToDoList_ListAPI;

namespace ToDoList_ListTest
{
    [TestFixture]
    public class ListTaskControllerTests
    {
        private ListTaskController _listTaskController;
        private Mock<IListTaskRepository> _listTaskRepoMock;
        private DataFactory _DataFactory;
        private ApplicationDbContext _dbContext;
        private IMapper _mapper;

        [SetUp]
        public void Setup()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("TestDatabase")
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            _dbContext = new ApplicationDbContext(options);

            _listTaskRepoMock = new Mock<IListTaskRepository>();
            _DataFactory = new DataFactory(_dbContext);

            // Initialize IMapper
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingConfig>()).CreateMapper();

            _listTaskController = new ListTaskController(_listTaskRepoMock.Object, _mapper);
        }

        [Test]
        [TestCase("Title1", HttpStatusCode.BadRequest, false)]
        public async Task GetListTasks_ValidRequest_ReturnsOkResult(string title, HttpStatusCode expectedStatusCode, bool expectedResult)
        {
            // Arrange
            var expectedList = _DataFactory.CreateTestListTasks();
            _listTaskRepoMock.Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<ListTask, bool>>>(), null, 0, 1))
                .ReturnsAsync(expectedList);

            // Act
            ActionResult<APIResponse> actionResult = await _listTaskController.GetListTasks(null, null, 0, 1);

            // Extract the APIResponse from the ActionResult
            APIResponse result = actionResult.Value;
            // Assert
            Assert.NotNull(actionResult);
            Assert.NotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(expectedStatusCode));
            Assert.That((result.IsSuccess), Is.EqualTo(expectedResult));
        }

        //[Test]
        //public async Task GetListTask_ValidId_ReturnsOkResult()
        //{
        //    // Arrange
        //    var expectedId = 1;
        //    var expectedResult = _DataFactory.CreateTestListTasks().FirstOrDefault();
        //    _listTaskRepoMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<ListTask, bool>>>(), true, null))
        //        .ReturnsAsync(expectedResult);

        //    // Act
        //    var result = await _listTaskController.GetListTask(expectedId);

        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.AreEqual((int)HttpStatusCode.OK, result.StatusCode);
        //    Assert.That((result.Value as APIResponse)?.Result, Is.EqualTo(expectedResult));
        //}
    }
}
