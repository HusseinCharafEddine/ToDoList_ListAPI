using NUnit.Framework;
using Moq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ToDoList_ListAPI.Controllers;
using ToDoList_Utility.Models.DTO;
using ToDoList_Utility.Models;
using ToDoList_Repository.Repository.IRepository;

namespace ToDoList_ListAPI.Tests
{
    [TestFixture]
    public class UserControllerTests
    {
        private UserController _userController;
        private Mock<IUserRepository> _userRepoMock;

        [SetUp]
        public void Setup()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _userController = new UserController(_userRepoMock.Object);
        }

        [Test]
        [TestCase("husseinsh", "test1234", HttpStatusCode.OK, true)]
        [TestCase("hussseinsh", "test12345", HttpStatusCode.BadRequest, false)]
        [TestCase("husseinsh3", "test12345", HttpStatusCode.BadRequest, false)]

        public async Task Login(string username, string password, HttpStatusCode expectedStatusCode, bool expectedIsSuccess)
        {
            // Arrange
            var loginRequest = new LoginRequestDTO
            {
                UserName = username,
                Password = password
            };

            var loginResponse = new LoginResponseDTO
            {
                Token = "asd",
                User = new LocalUser { /* user details */ }
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
        public async Task Register(string username, string password, HttpStatusCode expectedStatusCode, bool expectedIsSuccess, bool isUserNameUnique)
        {
            // Arrange
            var registrationRequest = new RegistrationRequestDTO
            {
                UserName = username,
                Password = password
            };

            _userRepoMock.Setup(repo => repo.IsUniqueUser(registrationRequest.UserName)).Returns(isUserNameUnique);
            _userRepoMock.Setup(repo => repo.Register(registrationRequest)).ReturnsAsync(new LocalUser { /* user details */ });

            // Act
            var result = await _userController.Register(registrationRequest) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo((int)expectedStatusCode));
            Assert.That((bool)result.Value.GetType().GetProperty("IsSuccess")?.GetValue(result.Value), Is.EqualTo(expectedIsSuccess));
        }

        [Test]
        [TestCase("existingemail@example.com", HttpStatusCode.OK, true)]
        [TestCase("nonexistentemail@example.com", HttpStatusCode.BadRequest, false)]
        public async Task ForgotPassword(string email, HttpStatusCode expectedStatusCode, bool expectedIsSuccess)
        {
            // Arrange
            var forgotPasswordRequest = new ForgotPasswordRequestDTO
            {
                Email = email
            };

            var forgotPasswordResponse = new ForgotPasswordResponseDTO
            {
                User = new LocalUser { /* user details */ },
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
    }
}