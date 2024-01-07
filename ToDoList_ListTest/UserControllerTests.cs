using NUnit.Framework;
using Moq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ToDoList_ListAPI.Controllers;
using ToDoList_Utility.Models.DTO;
using ToDoList_Repository.Repository.IRepository;
using ToDoList_Utility.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ToDoList_Repository.Data;
using System.Data;

namespace ToDoList_ListTest
{


    [TestFixture]
    public class UserControllerTests
    {
        private UserController _userController;
        private Mock<IUserRepository> _userRepoMock;
        private DataFactory _DataFactory;
        private ApplicationDbContext _dbContext;

        [SetUp]
        public void Setup()
        {
            // Set up an in-memory DbContext
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("TestDatabase")
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            _dbContext = new ApplicationDbContext(options);

            _userRepoMock = new Mock<IUserRepository>();
            _DataFactory = new DataFactory(_dbContext);
            _userController = new UserController(_userRepoMock.Object);
        }

        [Test]
        [TestCase("husseinsh", "test1234", "admin", "Hussein Sharaf Eddine", "hu.sharafeddine@gmail.com", HttpStatusCode.OK, true)]

        public async Task Login_ValidCredentials_ValidToken(string username, string password, string role, string name, string email, HttpStatusCode expectedStatusCode, bool expectedIsSuccess)
        {
            // Arrange
            var loginRequest = new LoginRequestDTO
            {
                UserName = username,
                Password = password
            };

            var testUser = _DataFactory.CreateTestUser(username, password, role, name, email);

            var loginResponse = new LoginResponseDTO
            {
                Token = "validToken",
                User = testUser
            };

            _userRepoMock.Setup(repo => repo.Login(loginRequest)).ReturnsAsync(loginResponse);

            // Act
            var result = await _userController.Login(loginRequest) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo((int)expectedStatusCode));
            Assert.AreEqual(expectedIsSuccess, (bool)result.Value.GetType().GetProperty("IsSuccess")?.GetValue(result.Value));
        }
        [TestCase("husseinsh", "test1234", "admin", "Hussein Sharaf Eddine", "hu.sharafeddine@gmail.com", HttpStatusCode.BadRequest, false)]
        public async Task Login_ValidCredentials_InvalidToken(string username, string password, string role, string name, string email, HttpStatusCode expectedStatusCode, bool expectedIsSuccess)
        {
            // Arrange
            var loginRequest = new LoginRequestDTO
            {
                UserName = username,
                Password = password
            };

            var testUser = _DataFactory.CreateTestUser(username, password, role, name, email);

            var loginResponse = new LoginResponseDTO
            {
                Token = "",
                User = testUser
            };

            _userRepoMock.Setup(repo => repo.Login(loginRequest)).ReturnsAsync(loginResponse);

            // Act
            var result = await _userController.Login(loginRequest) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo((int)expectedStatusCode));
            Assert.AreEqual(expectedIsSuccess, (bool)result.Value.GetType().GetProperty("IsSuccess")?.GetValue(result.Value));
        }
        [Test]
        [TestCase("newuser", "password", HttpStatusCode.OK, true, true)]
        [TestCase("existinguser", "password", HttpStatusCode.BadRequest, false, false)]
        public async Task Register_UniqueUsername_ReturnsExpectedResult(string username, string password, HttpStatusCode expectedStatusCode, bool expectedIsSuccess, bool isUserNameUnique)
        {
            var registrationRequest = new RegistrationRequestDTO
            {
                UserName = username,
                Password = password
            };

            _userRepoMock.Setup(repo => repo.IsUniqueUser(registrationRequest.UserName)).Returns(isUserNameUnique);
            _userRepoMock.Setup(repo => repo.Register(registrationRequest)).ReturnsAsync(new LocalUser { /* user details */ });

            var result = await _userController.Register(registrationRequest) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo((int)expectedStatusCode));
            Assert.That((bool)result.Value.GetType().GetProperty("IsSuccess")?.GetValue(result.Value), Is.EqualTo(expectedIsSuccess));
        }

        [Test]
        [TestCase("husseinsh", "test1234", "admin", "Hussein Sharaf Eddine", "hu.sharafeddine@gmail.com", HttpStatusCode.OK, true)]
        public async Task ForgotPassword_ExistingEmail(string username, string password, string role, string name, string email, HttpStatusCode expectedStatusCode, bool expectedIsSuccess)
        {
            // Arrange
            var forgotPasswordRequest = new ForgotPasswordRequestDTO
            {
                Email = email
            };
            var testUser = _DataFactory.CreateTestUser(username, password, role, name, email);

            var forgotPasswordResponse = new ForgotPasswordResponseDTO
            {
                User = testUser,
                Email = email
            };

            _userRepoMock.Setup(repo => repo.ForgotPassword(forgotPasswordRequest)).ReturnsAsync(forgotPasswordResponse);

            // Act
            var result = await _userController.ForgotPassword(forgotPasswordRequest) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo((int)expectedStatusCode));
            Assert.AreEqual(expectedIsSuccess, (bool)result.Value.GetType().GetProperty("IsSuccess")?.GetValue(result.Value));
        }
        [TestCase("husseinsh234234234", "test122434234", "user", "Hussein Sharaf Edsdfsdine", "hu.sfasdfasdf@gmail.com", HttpStatusCode.BadRequest, false)]
        public async Task ForgotPassword_NonExistingEmail(string username, string password, string role, string name, string email, HttpStatusCode expectedStatusCode, bool expectedIsSuccess)
        {
            // Arrange
            var forgotPasswordRequest = new ForgotPasswordRequestDTO
            {
                Email = email
            };
            var testUser = _DataFactory.CreateTestUser(username, password, role, name, email);

            var forgotPasswordResponse = new ForgotPasswordResponseDTO
            {
                User = null,
                Email = email
            };

            _userRepoMock.Setup(repo => repo.ForgotPassword(forgotPasswordRequest)).ReturnsAsync(forgotPasswordResponse);

            // Act
            var result = await _userController.ForgotPassword(forgotPasswordRequest) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo((int)expectedStatusCode));
            Assert.AreEqual(expectedIsSuccess, (bool)result.Value.GetType().GetProperty("IsSuccess")?.GetValue(result.Value));
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }
    }
}