using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using BookApp.Controllers;
using ModelLayer.DTO;

namespace AuthControllerTests
{
    [TestFixture]
    public class AuthControllerUnitTests
    {
        private Mock<IAuthBL> _mockAuthBL;
        private Mock<IForgotPasswordService> _mockForgotPasswordService;
        private AuthController _authController;

        [SetUp]
        public void Setup()
        {
            _mockAuthBL = new Mock<IAuthBL>();
            _mockForgotPasswordService = new Mock<IForgotPasswordService>();

            _authController = new AuthController(_mockAuthBL.Object, _mockForgotPasswordService.Object);
        }

        // ✅ Test Register User
        [Test]
        public async Task Register_ValidUser_ReturnsToken()
        {
            // Arrange
            var registerDto = new RegisterDTO { Email = "test@example.com", Password = "Secure123" };
            _mockAuthBL.Setup(a => a.RegisterAsync(registerDto)).ReturnsAsync("fake-jwt-token");

            // Act
            var result = await _authController.Register(registerDto) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsTrue(result.Value.ToString().Contains("token"));
        }

        [Test]
        public async Task Register_ExistingUser_ReturnsBadRequest()
        {
            // Arrange
            var registerDto = new RegisterDTO { Email = "existing@example.com", Password = "Password123" };
            _mockAuthBL.Setup(a => a.RegisterAsync(registerDto)).ReturnsAsync("User already exists");

            // Act
            var result = await _authController.Register(registerDto) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
        }

        // ✅ Test Login User
        [Test]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var loginDto = new LoginDTO { Username = "test", Password = "Secure123" };
            _mockAuthBL.Setup(a => a.LoginAsync(loginDto)).ReturnsAsync("fake-jwt-token");

            // Act
            var result = await _authController.Login(loginDto) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsTrue(result.Value.ToString().Contains("token"));
        }

        [Test]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginDto = new LoginDTO { Username = "wrong", Password = "WrongPass" };
            _mockAuthBL.Setup(a => a.LoginAsync(loginDto)).ReturnsAsync((string)null);

            // Act
            var result = await _authController.Login(loginDto) as UnauthorizedObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(401, result.StatusCode);
        }

        // ✅ Test Forgot Password
        [Test]
        public async Task ForgotPassword_ValidEmail_ReturnsSuccess()
        {
            // Arrange
            var dto = new ForgotPasswordDTO { Email = "valid@example.com" };
            _mockForgotPasswordService.Setup(f => f.ForgotPasswordAsync(dto)).ReturnsAsync(true);

            // Act
            var result = await _authController.ForgotPassword(dto) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual("Reset password email sent", result.Value.GetType().GetProperty("message").GetValue(result.Value, null));
        }

        [Test]
        public async Task ForgotPassword_InvalidEmail_ReturnsBadRequest()
        {
            // Arrange
            var dto = new ForgotPasswordDTO { Email = "invalid@example.com" };
            _mockForgotPasswordService.Setup(f => f.ForgotPasswordAsync(dto)).ReturnsAsync(false);

            // Act
            var result = await _authController.ForgotPassword(dto) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("Email not found", result.Value.GetType().GetProperty("message").GetValue(result.Value, null));
        }

        // ✅ Test Reset Password
        [Test]
        public async Task ResetPassword_ValidToken_ReturnsSuccess()
        {
            // Arrange
            var dto = new ResetPasswordDTO { Token = "valid-token", NewPassword = "NewPass123" };
            _mockForgotPasswordService.Setup(f => f.ResetPasswordAsync(dto)).ReturnsAsync(true);

            // Act
            var result = await _authController.ResetPassword(dto) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual("Password reset successful", result.Value.GetType().GetProperty("message").GetValue(result.Value, null));
        }

        [Test]
        public async Task ResetPassword_InvalidToken_ReturnsBadRequest()
        {
            // Arrange
            var dto = new ResetPasswordDTO { Token = "invalid-token", NewPassword = "NewPass123" };
            _mockForgotPasswordService.Setup(f => f.ResetPasswordAsync(dto)).ReturnsAsync(false);

            // Act
            var result = await _authController.ResetPassword(dto) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("Invalid token or expired", result.Value.GetType().GetProperty("message").GetValue(result.Value, null));
        }
    }
}
