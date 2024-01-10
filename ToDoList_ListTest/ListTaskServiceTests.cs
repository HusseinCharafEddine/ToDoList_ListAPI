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
using ToDoList_Repository.Data;
using ToDoList_Utility.Models.DTO;
using ToDoList_Utility.Models;
using ToDoList_Repository.Repository.IRepository;
using ToDoList_Repository.Repository;
using Microsoft.VisualBasic;
using NUnit.Framework.Internal;
using ToDoList_Services.Services.IServices;
using ToDoList_Services.Services;
using ToDoList_Services;
using ToDoList_Utility.Validators;
using ToDoList_ListAPI.Validators;
using ToDoList_Utility.Models.Exceptions;

namespace ToDoList_ListTest
{
    internal class ListTaskServiceTests
    {
        private IListTaskService _listTaskService;
        private IListTaskRepository _listTaskRepo;
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

            _listTaskRepo = new ListTaskRepository(_dbContext);
            _DataFactory = new DataFactory(_dbContext);
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingConfig>()).CreateMapper();
            _listTaskService = new ListTaskService(_listTaskRepo, _mapper) ;

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
            Assert.That(result.Count, Is.EqualTo(6));
        }
        [Test]
        [TestCase(1, 3)]
        public async Task GetAllAsync_WithValidInput_ReturnsMappedListTaskDTOList_WithValidPagination(int pageNumber, int pageSize)
        {
            // Arrange
            var filteredList = _DataFactory.CreateTestListTasks().AsQueryable();
            // Act
            var result = await _listTaskService.GetAllAsync(pageNumber: pageNumber, pageSize: pageSize);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<ListTaskDTO>>(result);
            Assert.That(result.Count, Is.EqualTo(3));
        }
        [Test]
        [TestCase(-1, 3)]
        [TestCase(3, -1)]
        public async Task GetAllAsync_WithValidInput_ReturnsMappedListTaskDTOList_WithInvalidPagination(int pageNumber, int pageSize)
        {
            // Arrange
            var filteredList = _DataFactory.CreateTestListTasks().AsQueryable();
            // Act and Assert
            var exception = Assert.ThrowsAsync<BadRequestException>(async () =>
            {
                await _listTaskService.GetAllAsync(pageNumber: pageNumber, pageSize: pageSize);
            });

            // Assert
            Assert.That(exception.Message, Does.Contain("nonpositive"));

        }

        [Test]
        [TestCase(1)]
        public async Task GetAsync_WithValidInput_ReturnsListTaskDTO(int id)
        {
            //Arrange
            var filteredList = _DataFactory.CreateTestListTasks().AsQueryable();
            //Act
            var result = await _listTaskService.GetAsync(id);

            //Assert

            Assert.IsNotNull(result);
            Assert.That(result.Id, Is.EqualTo(id));
        }
        [Test]
        [TestCase(7)]
        public async Task GetAsync_WithNonExistentID_ReturnsNull(int id)
        {
            //Arrange
            var filteredList = _DataFactory.CreateTestListTasks().AsQueryable();
            List<ListTaskDTO> test = _mapper.Map<List<ListTaskDTO>>(filteredList);
            //Act
            var exception = Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                await _listTaskService.GetAsync(id);
            });

            // Assert
            Assert.That(exception.Message, Does.Contain("Task has not been found"));
        }
        [Test]
        [TestCase(0)]
        [TestCase(-2)]
        public async Task GetAsync_WithNegativeID_ReturnsNUll(int id)
        {
            //Arrange
            var filteredList = _DataFactory.CreateTestListTasks().AsQueryable();
            List<ListTaskDTO> test = _mapper.Map<List<ListTaskDTO>>(filteredList);
            //Act
            var exception = Assert.ThrowsAsync<BadRequestException>(async () =>
            {
                await _listTaskService.GetAsync(id);
            });

            // Assert
            Assert.That(exception.Message, Does.Contain("Id can not be nonpositive"));
        }

        [Test]
        [TestCase(6, "hello", "safd", "asdf", true, ExpectedResult = 6)]

        public async Task<int> CreateTestListTaskAsync_ValidInput(int id, string title, string category, string description, bool isCompleted)
        {
            //Arrange
            var filteredList = _DataFactory.CreateTestListTasks().AsQueryable();
            var listTask = new ListTaskCreateDTO
            {
                Title = title,
                Category = category,
                Description = description,
                DueDate = DateTime.Now,
                IsCompleted = isCompleted
            };
            //Act
            await _listTaskService.CreateAsync(listTask);
            var listTasks = await _listTaskService.GetAllAsync();
            var task = await _listTaskService.GetAsync(id);
            // Assert
            Assert.IsNotNull(task);
            return task.Id;
        }
        [Test]
        [TestCase(7, null, "safd", "asdf", true)]
        public async Task CreateTestListTaskAsync_WithNullTitle(int id, string title, string category, string description, bool isCompleted)
        {
            //Arrange
            var filteredList = _DataFactory.CreateTestListTasks().AsQueryable();
            var listTask = new ListTaskCreateDTO
            {
                Id = id,
                Title = title,
                Category = category,
                Description = description,
                DueDate = DateTime.Now,
                IsCompleted = isCompleted
            };
            //Act and Assert
            var exception = Assert.ThrowsAsync<BadRequestException>(async () =>
            {
                await _listTaskService.CreateAsync(listTask);
            });

            // Assert
            Assert.That(exception.Message, Does.Contain("Title can not be empty."));
        }
        [Test]
        public async Task CreateTestListTaskAsync_WithNullDTO()
        {
            var filteredList = _DataFactory.CreateTestListTasks().AsQueryable();
            var exception = Assert.ThrowsAsync<BadRequestException>(async () =>
            {
                await _listTaskService.CreateAsync(null);
            });

            // Assert
            Assert.That(exception.Message, Does.Contain("Some fields"));
        }

        [Test]
        public async Task DeleteListTaskAsync_ValidInput()
        {
            int id = CreateTestListTaskAsync_ValidInput(6, "hello", "safd", "asdf", true).Result;
            var listTaskPreDelete = await _listTaskService.GetAsync(id);
            await _listTaskService.RemoveAsync(listTaskPreDelete.Id);
            GetAsync_WithNonExistentID_ReturnsNull(listTaskPreDelete.Id);
            Assert.IsNotNull(listTaskPreDelete);
        }
        [Test]
        [TestCase(7)]
        public async Task DeleteListTaskAsync_NonExistentId(int id)
        {
            //Arrange
            _DataFactory.CreateTestListTasks();
            //Act and Assert
            var exception = Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                await _listTaskService.RemoveAsync(id);
            });
            //Assert
            Assert.That(exception.Message, Does.Contain("Task has not been found"));

        }
        [Test]
        [TestCase(0)]
        [TestCase(-2)]
        public async Task DeleteListTaskAsync_NonPositiveId(int id)
        {
            //Arrange
            _DataFactory.CreateTestListTasks();
            //Act and Assert
            var exception = Assert.ThrowsAsync<BadRequestException>(async () =>
            {
                await _listTaskService.RemoveAsync(id);
            });
            //Assert
            Assert.That(exception.Message, Does.Contain("Id can not be nonpositive"));
        }
        [Test]
        public async Task UpdateListTaskAsync_ValidId_ValidDTO()
        {
            //Arrange
            int id = CreateTestListTaskAsync_ValidInput(6, "hello", "safd", "asdf", true).Result;
            ListTaskUpdateDTO listTaskUpdateDTO = _DataFactory.CreateTestListTaskUpdateDTO(6);
            //Act
            var listTaskPreUpdate = await _listTaskService.GetAsync(id);
            var existingEntity = _dbContext.Set<ListTask>().Local.FirstOrDefault(u => u.Id == id);
            if (existingEntity != null)
            {
                _dbContext.Entry(existingEntity).State = EntityState.Detached;
            }
            await _listTaskService.UpdateAsync(id, listTaskUpdateDTO);
            var listTaskPostUpdate = await _listTaskService.GetAsync(id);

            //Assert
            Assert.That(listTaskPreUpdate.Id, Is.EqualTo(listTaskPostUpdate.Id));
            Assert.That(listTaskPreUpdate.Title, Is.Not.EqualTo(listTaskPostUpdate.Title));

        }



        [Test]
        public async Task UpdateListTaskAsync_NonExistentId_ValidDTO() {
            int id = CreateTestListTaskAsync_ValidInput(6, "hello", "safd", "asdf", true).Result;
            ListTaskUpdateDTO listTaskUpdateDTO = _DataFactory.CreateTestListTaskUpdateDTO(7);


            //Act
            var exception = Assert.ThrowsAsync<BadRequestException>(async () =>
            {
                await _listTaskService.UpdateAsync(id, listTaskUpdateDTO);
            });

            //Assert
            Assert.That(exception.Message, Does.Contain("Task is null, or id toesnt match that of the task"));
        }

        [Test]
        public async Task UpdateListTaskAsync_NonPositiveId_ValidDTO()
        {
            int id = CreateTestListTaskAsync_ValidInput(6, "hello", "safd", "asdf", true).Result;
            ListTaskUpdateDTO listTaskUpdateDTO = _DataFactory.CreateTestListTaskUpdateDTO(-2);


            //Act

            var exception = Assert.ThrowsAsync<BadRequestException>(async () =>
            {
                await _listTaskService.UpdateAsync(-2, listTaskUpdateDTO);
            });

            //Assert
            Assert.That(exception.Message, Does.Contain("Id can not be nonpositive"));
        }
        [Test]

        public async Task UpdateListTaskAsync_ValidId_UnvalidDTO()
        {
            //Arrange

            int id = CreateTestListTaskAsync_ValidInput(6, "hello", "safd", "asdf", true).Result;
            ListTaskUpdateDTO listTaskUpdateDTO = _DataFactory.CreateTestListTaskUpdateDTONullTitle(6);

            //Act
            var listTaskPreUpdate = await _listTaskService.GetAsync(id);
            var existingEntity = _dbContext.Set<ListTask>().Local.FirstOrDefault(u => u.Id == id);
            if (existingEntity != null)
            {
                _dbContext.Entry(existingEntity).State = EntityState.Detached;
            }
            var exception = Assert.ThrowsAsync<BadRequestException>(async () =>
            {
                await _listTaskService.UpdateAsync(id, listTaskUpdateDTO);
            });
            var listTaskPostUpdate = await _listTaskService.GetAsync(id);

            //Assert
            Assert.That(listTaskPreUpdate.Id, Is.EqualTo(listTaskPostUpdate.Id));
            Assert.That(listTaskPreUpdate.Title, Is.EqualTo(listTaskPostUpdate.Title));
            Assert.That(exception.Message, Does.Contain("Title can not be empty"));

        }
        [Test]

        public async Task UpdateListTaskAsync_NonExistentId_UnvalidDTO_MismatchingIds()
        {
            int id = CreateTestListTaskAsync_ValidInput(5, "hello", "safd", "asdf", true).Result;
            ListTaskUpdateDTO listTaskUpdateDTO = _DataFactory.CreateTestListTaskUpdateDTONullTitle(6);

            //Act
            GetAsync_WithNonExistentID_ReturnsNull(listTaskUpdateDTO.Id);
            var exception = Assert.ThrowsAsync<BadRequestException>(async () =>
            {
                await _listTaskService.UpdateAsync(id, listTaskUpdateDTO);
            });

            //Assert
            Assert.That(exception.Message, Does.Contain("Task is null, or id toesnt match that of the task"));
        }
        [Test]
        public async Task UpdateListTaskAsync_NonExistentId_UnvalidDTO_MatchingIds()
        {
            int id = CreateTestListTaskAsync_ValidInput(6, "hello", "safd", "asdf", true).Result;
            ListTaskUpdateDTO listTaskUpdateDTO = _DataFactory.CreateTestListTaskUpdateDTONullTitle(6);


            //Act
            GetAsync_WithNonExistentID_ReturnsNull(listTaskUpdateDTO.Id);
            var exception = Assert.ThrowsAsync<BadRequestException>(async () =>
            {
                await _listTaskService.UpdateAsync(id, listTaskUpdateDTO);
            });

            //Assert
            Assert.That(exception.Message, Does.Contain("Title can not be empty"));
        }

        [Test]
        [TestCase( 2)]
        [TestCase( -1)]

        public async Task UpdateListTaskAsync_NonPositiveId_UnvalidDTO( int id2)
        {
            int id = CreateTestListTaskAsync_ValidInput(6, "hello", "safd", "asdf", true).Result;
            ListTaskUpdateDTO listTaskUpdateDTO = _DataFactory.CreateTestListTaskUpdateDTONullTitle(id2);


            //Act
            var exception = Assert.ThrowsAsync<BadRequestException>(async () =>
            {
                await _listTaskService.UpdateAsync(-1, listTaskUpdateDTO);
            });

            //Assert
            Assert.That(exception.Message, Does.Contain("Id can not be nonpositive"));
        }
        [Test]

        public async Task UpdateListTaskAsync_ValidId_NullDTO()
        {
            //Arrange

            int id = CreateTestListTaskAsync_ValidInput(6, "hello", "safd", "asdf", true).Result;

            //Act
            var listTaskPreUpdate = await _listTaskService.GetAsync(id);
            var existingEntity = _dbContext.Set<ListTask>().Local.FirstOrDefault(u => u.Id == id);
            if (existingEntity != null)
            {
                _dbContext.Entry(existingEntity).State = EntityState.Detached;
            }
            var exception = Assert.ThrowsAsync<BadRequestException>(async () =>
            {
                await _listTaskService.UpdateAsync(id, null);
            });
            var listTaskPostUpdate = await _listTaskService.GetAsync(id);

            //Assert
            Assert.That(listTaskPreUpdate.Id, Is.EqualTo(listTaskPostUpdate.Id));
            Assert.That(listTaskPreUpdate.Title, Is.EqualTo(listTaskPostUpdate.Title));
            Assert.That(exception.Message, Does.Contain("Task is null"));

        }
        [Test]
        public async Task UpdateListTaskAsync_NonExistentId_NullDTO()
        {

            int id = CreateTestListTaskAsync_ValidInput(6, "hello", "safd", "asdf", true).Result;

            //Act
            GetAsync_WithNonExistentID_ReturnsNull(7);
            var exception = Assert.ThrowsAsync<BadRequestException>(async () =>
            {
                await _listTaskService.UpdateAsync(7, null);
            });

            //Assert
            Assert.That(exception.Message, Does.Contain("Task is null, or id toesnt match that of the task"));
        }

        [Test]
        [TestCase(-1)]
        public async Task UpdateListTaskAsync_NonPositiveId_NullDTO(int id)
        {
            ListTaskUpdateDTO listTaskUpdateDTO = null;

            _DataFactory.CreateTestListTasks();

            //Act
            var exception = Assert.ThrowsAsync<BadRequestException>(async () =>
            {
                await _listTaskService.UpdateAsync(id, listTaskUpdateDTO);
            });

            //Assert
            Assert.That(exception.Message, Does.Contain("Id can not be nonpositive"));
        }

        [Test]

        public async Task UpdateListTaskAsync_ValidId_ValidDTO_MismatchingIds()
        {
            //Arrange

            int id = CreateTestListTaskAsync_ValidInput(3, "hello", "safd", "asdf", true).Result;
            ListTaskUpdateDTO listTaskUpdateDTO = _DataFactory.CreateTestListTaskUpdateDTO(2);

            //Act
            var listTaskPreUpdate = await _listTaskService.GetAsync(id);
            var existingEntity = _dbContext.Set<ListTask>().Local.FirstOrDefault(u => u.Id == id);
            if (existingEntity != null)
            {
                _dbContext.Entry(existingEntity).State = EntityState.Detached;
            }
            var exception = Assert.ThrowsAsync<BadRequestException>(async () =>
            {
                await _listTaskService.UpdateAsync(id, listTaskUpdateDTO);
            });
            var listTaskPostUpdate = await _listTaskService.GetAsync(id);

            //Assert
            Assert.That(exception.Message, Does.Contain("id toesnt match that of the task"));
            Assert.That(listTaskPreUpdate.Id, Is.EqualTo(listTaskPostUpdate.Id));
            Assert.That(listTaskPreUpdate.Title, Is.EqualTo(listTaskPostUpdate.Title));

        }
    }
}
