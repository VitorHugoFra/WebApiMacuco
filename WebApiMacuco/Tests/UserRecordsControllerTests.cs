using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.AutoMock;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiMacuco.Controllers;
using WebApiMacuco.Data;
using WebApiMacuco.Models;
using WebApiMacuco.Services;
using Xunit;

namespace WebApiMacuco.Tests
{
    public class UserRecordsControllerTests
    {
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly UserRecordsController _controller;
        private readonly AutoMocker _mocker;

        public UserRecordsControllerTests()
        {
            _mockContext = new Mock<ApplicationDbContext>();
            var configuration = new Mock<IConfiguration>();
            configuration.SetupGet(c => c["Jwt:Secret"]).Returns("your_secret_key");
            _controller = new UserRecordsController(_mockContext.Object, configuration.Object);
            _mocker = new AutoMocker();
        }

        [Fact]
        public void GetUserRecord_ReturnsOk()
        {
            var result = _controller.GetUserRecord(1);
            Assert.NotNull(result);
        }

        [Fact]
        public void UpdateUserRecord_ValidPatch_ReturnsOk()
        {
            var patchDoc = new JsonPatchDocument<UserRecord>();
            var result = _controller.UpdateUserRecord(1, patchDoc);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetUserRecords_ReturnsAllRecords()
        {
            var mockUserRecords = new List<UserRecord>
            {
                new UserRecord { UserId = 1, Description = "Face 1" },
                new UserRecord { UserId = 2, Description = "Face 2" }
            };
            _mockContext.Setup(c => c.UserRecords.ToListAsync(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(mockUserRecords);

            var result = await _controller.GetUserRecords();

            var actionResult = Assert.IsType<ActionResult<IEnumerable<UserRecord>>>(result);
            var returnValue = Assert.IsType<List<UserRecord>>(actionResult.Value);
            Assert.Equal(2, returnValue.Count);
        }


        [Fact]
        public async Task GetUserRecord_ReturnsRecordById()
        {
            var mockUserRecord = new UserRecord { UserId = 1, Description = "Face 1" };
            _mockContext.Setup(c => c.UserRecords.FindAsync(1)).ReturnsAsync(mockUserRecord);

            var result = _controller.GetUserRecord(1);

            var actionResult = Assert.IsType<ActionResult<UserRecord>>(result);
            var returnValue = Assert.IsType<UserRecord>(actionResult.Value);
            Assert.Equal(mockUserRecord.UserId, returnValue.UserId);

            await Task.CompletedTask;
        }


        [Fact]
        public async Task PostUserRecord_CreatesNewRecord()
        {
            var newUserRecord = new UserRecord { UserId = 3, Description = "Face 3" };

            var result = await _controller.PostUserRecord(newUserRecord);

            var actionResult = Assert.IsType<ActionResult<UserRecord>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsType<UserRecord>(createdAtActionResult.Value);
            Assert.Equal(newUserRecord.UserId, returnValue.UserId);
        }

        [Fact]
        public async Task PutUserRecord_UpdatesRecord()
        {
            var mockUserRecord = new UserRecord { UserId = 1, Description = "Face 1" };
            _mockContext.Setup(c => c.UserRecords.FindAsync(1)).ReturnsAsync(mockUserRecord);
            _mockContext.Setup(c => c.Entry(mockUserRecord).State).Returns(EntityState.Modified);
            var updatedUserRecord = new UserRecord { UserId = 1, Description = "Updated Face 1" };

            var result = await _controller.PutUserRecord(1, updatedUserRecord);

            Assert.IsType<NoContentResult>(result);
            Assert.Equal("Updated Face 1", mockUserRecord.Description);
        }


        [Fact]
        public async Task DeleteUserRecord_RemovesRecord()
        {
            // Arrange
            var mockUserRecord = new UserRecord { UserId = 1, Description = "Face 1" };
            _mockContext.Setup(c => c.UserRecords.FindAsync(It.IsAny<int>()))
                        .ReturnsAsync((int id) => mockUserRecord);

            // Act
            var result = await _controller.DeleteUserRecord(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockContext.Verify(c => c.UserRecords.Remove(mockUserRecord), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }



    }
}
