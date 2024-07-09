using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
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

    public UserRecordsControllerTests()
    {
      var mockConfiguration = new Mock<IConfiguration>();
      mockConfiguration.SetupGet(x => x["Jwt:Secret"]).Returns("AF4eJTYcxO");

      _mockContext = new Mock<ApplicationDbContext>();
      _controller = new UserRecordsController(_mockContext.Object, mockConfiguration.Object);
    }

    [Fact]
    public async Task GetUserRecords_ReturnsAllRecords()
    {
      // Arrange
      var mockUserRecords = new List<UserRecord>
            {
                new UserRecord { Id = 1, Description = "Face 1" },
                new UserRecord { Id = 2, Description = "Face 2" }
            };
      _mockContext.Setup(c => c.UserRecords.ToListAsync()).ReturnsAsync(mockUserRecords);

      // Act
      var result = await _controller.GetUserRecords();

      // Assert
      var actionResult = Assert.IsType<ActionResult<IEnumerable<UserRecord>>>(result);
      var returnValue = Assert.IsType<List<UserRecord>>(actionResult.Value);
      Assert.Equal(2, returnValue.Count);
    }

    [Fact]
    public async Task GetUserRecord_ReturnsRecordById()
    {
      // Arrange
      var mockUserRecord = new UserRecord { Id = 1, Description = "Face 1" };
      _mockContext.Setup(c => c.UserRecords.FindAsync(1)).ReturnsAsync(mockUserRecord);

      // Act
      var result = await _controller.GetUserRecord(1);

      // Assert
      var actionResult = Assert.IsType<ActionResult<UserRecord>>(result);
      var returnValue = Assert.IsType<UserRecord>(actionResult.Value);
      Assert.Equal(mockUserRecord.Id, returnValue.Id);
    }

    [Fact]
    public async Task PostUserRecord_CreatesNewRecord()
    {
      // Arrange
      var newUserRecord = new UserRecord { Id = 3, Description = "Face 3" };

      // Act
      var result = await _controller.PostUserRecord(newUserRecord);

      // Assert
      var actionResult = Assert.IsType<ActionResult<UserRecord>>(result);
      var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
      var returnValue = Assert.IsType<UserRecord>(createdAtActionResult.Value);
      Assert.Equal(newUserRecord.Id, returnValue.Id);
    }

    [Fact]
    public async Task PutUserRecord_UpdatesRecord()
    {
      // Arrange
      var mockUserRecord = new UserRecord { Id = 1, Description = "Face 1" };
      _mockContext.Setup(c => c.UserRecords.FindAsync(1)).ReturnsAsync(mockUserRecord);
      var updatedUserRecordDto = new FaceUpdateDto { Id = 1, Description = "Updated Face 1" };

      // Act
      var result = await _controller.PutUserRecord(1, updatedUserRecordDto);

      // Assert
      Assert.IsType<NoContentResult>(result);
      Assert.Equal("Updated Face 1", mockUserRecord.Description);
    }

    [Fact]
    public async Task DeleteUserRecord_RemovesRecord()
    {
      // Arrange
      var mockUserRecord = new UserRecord { Id = 1, Description = "Face 1" };
      _mockContext.Setup(c => c.UserRecords.FindAsync(1)).ReturnsAsync(mockUserRecord);

      // Act
      var result = await _controller.DeleteUserRecord(1);

      // Assert
      Assert.IsType<NoContentResult>(result);
      _mockContext.Verify(c => c.UserRecords.Remove(mockUserRecord), Times.Once);
      _mockContext.Verify(c => c.SaveChangesAsync(), Times.Once);
    }
  }
}
