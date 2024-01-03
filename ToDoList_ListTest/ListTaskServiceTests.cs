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
using ToDoList_ListAPI.Repository;
using Microsoft.VisualBasic;
using NUnit.Framework.Internal;

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
            _listTaskService = new ListTaskService(_listTaskRepo, _mapper);
            
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
        [TestCase(1,3)]
        public async Task GetAllAsync_WithValidInput_ReturnsMappedListTaskDTOList_WithValidPagination(int pageNumber,  int pageSize)
        {
            // Arrange
            var filteredList = _DataFactory.CreateTestListTasks().AsQueryable();
            // Act
            var result = await _listTaskService.GetAllAsync(pageNumber:pageNumber, pageSize: pageSize);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<ListTaskDTO>>(result);
            Assert.That(result.Count, Is.EqualTo(3));
        }
        [Test]
        [TestCase(-1, 3)]
        [TestCase(3,-1)]
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
            Assert.That(exception.Message, Does.Contain("can not be negative"));

        }

        [Test]
        [TestCase(1)]
        public async Task GetAsync_WithValidInput_ReturnsListTaskDTO (int id)
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
        [TestCase(7, null , "safd", "asdf", true)]
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
            Assert.That(exception.Message, Does.Contain("Some fields"));
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
        public async Task UpdateListTaskAsync_ValidId_ValidDTO ()
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
            Assert.That(listTaskPreUpdate.Title , Is.Not.EqualTo(listTaskPostUpdate.Title));    
        
        }

       

        [Test]
    [TestCase(7, 2, "title", "category", "description", true)]
    public async Task UpdateListTaskAsync_NonExistentId_ValidDTO(int id, int id2, string title, string category, string description, bool isCompleted) {
        var listTaskUpdateDTO = new ListTaskUpdateDTO
        {
            Id = id2,
            Title = title,
            Category = category,
            Description = description,
            DueDate = DateTime.Now,
            IsCompleted = isCompleted
        };
        _DataFactory.CreateTestListTasks();

            //Act
            var exception = Assert.ThrowsAsync<BadRequestException>(async () =>
            {
                await _listTaskService.UpdateAsync(id, listTaskUpdateDTO);
            });

            //Assert
            Assert.That(exception.Message, Does.Contain("Task is null, or id toesnt match that of the task"));
        }

        [Test]
        [TestCase(-1, 2, "title", "category", "description", true)]
        public async Task UpdateListTaskAsync_NonPositiveId_ValidDTO(int id, int id2, string title, string category, string description, bool isCompleted)
        {
            var listTaskUpdateDTO = new ListTaskUpdateDTO
            {
                Id = id2,
                Title = title,
                Category = category,
                Description = description,
                DueDate = DateTime.Now,
                IsCompleted = isCompleted
            };
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
        [TestCase(2, 2, "title", "category", "description", true)]

        public async Task UpdateListTaskAsync_ValidId_UnvalidDTO(int id, int id2, string title, string category, string description, bool isCompleted)
        {
            //Arrange

            var listTaskUpdateDTO = new ListTaskUpdateDTO
            {
                Id = id2,
                Title = null,
                Category = category,
                Description = description,
                DueDate = DateTime.Now,
                IsCompleted = isCompleted
            };
            _DataFactory.CreateTestListTasks();

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
        [TestCase(7, 2, "title", "category", "description", true)]

        public async Task UpdateListTaskAsync_NonExistentId_UnvalidDTO_MismatchingIds(int id, int id2, string title, string category, string description, bool isCompleted)
        {
            var listTaskUpdateDTO = new ListTaskUpdateDTO
            {
                Id = id2,
                Title = null,
                Category = category,
                Description = description,
                DueDate = DateTime.Now,
                IsCompleted = isCompleted
            };
            _DataFactory.CreateTestListTasks();

            //Act
            GetAsync_WithNonExistentID_ReturnsNull(listTaskUpdateDTO.Id);
            var exception = Assert.ThrowsAsync<BadRequestException>(async () =>
            {
                await _listTaskService.UpdateAsync(id, listTaskUpdateDTO);
            });

            //Assert
            Assert.That(exception.Message, Does.Contain("Task is null, or id toesnt match that of the task"));
        }
        [TestCase(7, 7, "title", "category", "description", true)]

        public async Task UpdateListTaskAsync_NonExistentId_UnvalidDTO_MatchingIds(int id, int id2, string title, string category, string description, bool isCompleted)
        {
            var listTaskUpdateDTO = new ListTaskUpdateDTO
            {
                Id = id2,
                Title = null,
                Category = category,
                Description = description,
                DueDate = DateTime.Now,
                IsCompleted = isCompleted
            };
            _DataFactory.CreateTestListTasks();

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
        [TestCase(-1, 2, "title", "category", "description", true)]
        [TestCase(-1, -1, "title", "category", "description", true)]

        public async Task UpdateListTaskAsync_NonPositiveId_UnvalidDTO(int id, int id2, string title, string category, string description, bool isCompleted)
        {
            var listTaskUpdateDTO = new ListTaskUpdateDTO
            {
                Id = id2,
                Title = null,
                Category = category,
                Description = description,
                DueDate = DateTime.Now,
                IsCompleted = isCompleted
            };
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
        [TestCase(2)]

        public async Task UpdateListTaskAsync_ValidId_NullDTO(int id)
        {
            //Arrange

            ListTaskUpdateDTO listTaskUpdateDTO = null;
            _DataFactory.CreateTestListTasks();

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
            Assert.That(exception.Message, Does.Contain("Task is null"));

        }
        [Test]
        [TestCase(7)]
        public async Task UpdateListTaskAsync_NonExistentId_NullDTO(int id)
        {
            ListTaskUpdateDTO listTaskUpdateDTO = null;

            _DataFactory.CreateTestListTasks();

            //Act
            GetAsync_WithNonExistentID_ReturnsNull(id);
            var exception = Assert.ThrowsAsync<BadRequestException>(async () =>
            {
                await _listTaskService.UpdateAsync(id, listTaskUpdateDTO);
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
        [TestCase(3, 2, "title", "category", "description", true)]

        public async Task UpdateListTaskAsync_ValidId_ValidDTO_MismatchingIds(int id, int id2, string title, string category, string description, bool isCompleted)
        {
            //Arrange

            var listTaskUpdateDTO = new ListTaskUpdateDTO
            {
                Id = id2,
                Title = title,
                Category = category,
                Description = description,
                DueDate = DateTime.Now,
                IsCompleted = isCompleted
            };
            _DataFactory.CreateTestListTasks();

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
