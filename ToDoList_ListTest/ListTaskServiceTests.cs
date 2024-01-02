using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList_ListAPI;
using ToDoList_ListAPI.Controllers;
using ToDoList_ListAPI.Data;
using ToDoList_ListAPI.Models.DTO;
using ToDoList_ListAPI.Models;
using ToDoList_ListAPI.Repository.IRepository;
using ToDoList_ListAPI.Services;
using ToDoList_ListAPI.Services.IServices;

namespace ToDoList_ListTest
{
    internal class ListTaskServiceTests
    {
        private IListTaskService _listTaskService;
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
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingConfig>()).CreateMapper();


            _listTaskService = new ListTaskService(_listTaskRepoMock.Object, _mapper);
        }
        [Test]
        public async Task GetAllAsync_WithValidInput_ReturnsMappedListTaskDTOList()
        {
            // Arrange
            var filteredList = _DataFactory.CreateTestListTasks().AsQueryable();
            // Act
            var result = await _listTaskService.GetAllAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<ListTaskDTO>>(result);
            Assert.That(result.Count, Is.EqualTo(1));
        }


    }
}
