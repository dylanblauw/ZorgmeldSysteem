using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using ZorgmeldSysteem.Application.DTOs.Object;
using ZorgmeldSysteem.Application.Interfaces.IServices;
using ZorgmeldSysteem.Domain.Enums;
using ZorgmeldSysteem.WebApi.Controllers;

namespace ZorgmeldSysteem.UnitTests.Controllers;

/// <summary>
/// Unit Tests voor ObjectController
/// Test alle endpoints voor object/apparaat beheer
/// </summary>
public class ObjectControllerTests
{
    #region GET Tests

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithListOfObjects()
    {
        // Arrange
        var mockService = new Mock<IObjectService>();
        var testObjects = new List<ObjectDto>
        {
            new ObjectDto { ObjectID = 1, ObjectCode = "OBJ-001", Name = "Lift A" },
            new ObjectDto { ObjectID = 2, ObjectCode = "OBJ-002", Name = "Lift B" }
        };
        mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(testObjects);
        var controller = new ObjectController(mockService.Object);

        // Act
        var result = await controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var objects = Assert.IsAssignableFrom<IEnumerable<ObjectDto>>(okResult.Value);
        Assert.Equal(2, objects.Count());
    }

    [Fact]
    public async Task GetById_ReturnsOkResult_WhenObjectExists()
    {
        // Arrange
        var mockService = new Mock<IObjectService>();
        var testObject = new ObjectDto
        {
            ObjectID = 1,
            ObjectCode = "OBJ-001",
            Name = "Test Lift",
            Brand = "OTIS",
            Model = "Model X",
            Location = "Gebouw A"
        };
        mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(testObject);
        var controller = new ObjectController(mockService.Object);

        // Act
        var result = await controller.GetById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var obj = Assert.IsType<ObjectDto>(okResult.Value);
        Assert.Equal(1, obj.ObjectID);
        Assert.Equal("OBJ-001", obj.ObjectCode);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenObjectDoesNotExist()
    {
        // Arrange
        var mockService = new Mock<IObjectService>();
        mockService.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((ObjectDto?)null);
        var controller = new ObjectController(mockService.Object);

        // Act
        var result = await controller.GetById(999);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Object met id 999 niet gevonden", notFoundResult.Value);
    }

    [Fact]
    public async Task GetByObjectCode_ReturnsOkResult_WhenObjectExists()
    {
        // Arrange
        var mockService = new Mock<IObjectService>();
        var testObject = new ObjectDto
        {
            ObjectID = 1,
            ObjectCode = "OBJ-001",
            Name = "Test Object"
        };
        mockService.Setup(s => s.GetByObjectCodeAsync("OBJ-001")).ReturnsAsync(testObject);
        var controller = new ObjectController(mockService.Object);

        // Act
        var result = await controller.GetByObjectCode("OBJ-001");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var obj = Assert.IsType<ObjectDto>(okResult.Value);
        Assert.Equal("OBJ-001", obj.ObjectCode);
    }

    [Fact]
    public async Task GetByObjectCode_ReturnsNotFound_WhenObjectDoesNotExist()
    {
        // Arrange
        var mockService = new Mock<IObjectService>();
        mockService.Setup(s => s.GetByObjectCodeAsync("OBJ-999")).ReturnsAsync((ObjectDto?)null);
        var controller = new ObjectController(mockService.Object);

        // Act
        var result = await controller.GetByObjectCode("OBJ-999");

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Object met code OBJ-999 niet gevonden", notFoundResult.Value);
    }

    [Fact]
    public async Task GetByCompanyId_ReturnsOkResult_WithObjectsForCompany()
    {
        // Arrange
        var mockService = new Mock<IObjectService>();
        var companyObjects = new List<ObjectDto>
        {
            new ObjectDto { ObjectID = 1, CompanyID = 5, Name = "Object 1" },
            new ObjectDto { ObjectID = 2, CompanyID = 5, Name = "Object 2" }
        };
        mockService.Setup(s => s.GetByCompanyIdAsync(5)).ReturnsAsync(companyObjects);
        var controller = new ObjectController(mockService.Object);

        // Act
        var result = await controller.GetByCompanyId(5);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var objects = Assert.IsAssignableFrom<IEnumerable<ObjectDto>>(okResult.Value);
        Assert.Equal(2, objects.Count());
        Assert.All(objects, o => Assert.Equal(5, o.CompanyID));
    }

    [Fact]
    public async Task GetLocationsByCompanyId_ReturnsOkResult_WithUniqueLocations()
    {
        // Arrange
        var mockService = new Mock<IObjectService>();
        var companyObjects = new List<ObjectDto>
        {
            new ObjectDto { ObjectID = 1, CompanyID = 5, Location = "Gebouw A" },
            new ObjectDto { ObjectID = 2, CompanyID = 5, Location = "Gebouw B" },
            new ObjectDto { ObjectID = 3, CompanyID = 5, Location = "Gebouw A" }, // Duplicate
            new ObjectDto { ObjectID = 4, CompanyID = 5, Location = "" } // Empty
        };
        mockService.Setup(s => s.GetByCompanyIdAsync(5)).ReturnsAsync(companyObjects);
        var controller = new ObjectController(mockService.Object);

        // Act
        var result = await controller.GetLocationsByCompanyId(5);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var locations = Assert.IsAssignableFrom<List<string>>(okResult.Value);
        Assert.Equal(2, locations.Count); // Alleen "Gebouw A" en "Gebouw B"
        Assert.Contains("Gebouw A", locations);
        Assert.Contains("Gebouw B", locations);
    }

    [Fact]
    public async Task GetDueForMaintenance_ReturnsOkResult_WithObjectsDueForMaintenance()
    {
        // Arrange
        var mockService = new Mock<IObjectService>();
        var objectsDue = new List<ObjectDto>
        {
            new ObjectDto { ObjectID = 1, Name = "Object 1", NextMaintenance = DateTime.Now.AddDays(-5) },
            new ObjectDto { ObjectID = 2, Name = "Object 2", NextMaintenance = DateTime.Now.AddDays(2) }
        };
        mockService.Setup(s => s.GetDueForMaintenanceAsync()).ReturnsAsync(objectsDue);
        var controller = new ObjectController(mockService.Object);

        // Act
        var result = await controller.GetDueForMaintenance();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var objects = Assert.IsAssignableFrom<IEnumerable<ObjectDto>>(okResult.Value);
        Assert.Equal(2, objects.Count());
    }

    #endregion

    #region POST Tests

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WhenSuccessful()
    {
        // Arrange
        var mockService = new Mock<IObjectService>();
        var createDto = new CreateObjectDto
        {
            ObjectCode = "OBJ-001",
            Name = "Nieuwe Lift",
            Description = "Test lift",
            Location = "Gebouw A",
            Brand = "OTIS",
            Model = "Model X",
            SerialNumber = "SN12345",
            CompanyID = 1,
            CreatedBy = "TestUser",
            DefaultPriority = Priority.Normal,
            DefaultReactionTime = ReactionTime.Within24Hours
        };

        var createdObject = new ObjectDto
        {
            ObjectID = 1,
            ObjectCode = createDto.ObjectCode,
            Name = createDto.Name,
            CompanyID = createDto.CompanyID
        };

        mockService.Setup(s => s.CreateAsync(It.IsAny<CreateObjectDto>()))
            .ReturnsAsync(createdObject);
        var controller = new ObjectController(mockService.Object);

        // Act
        var result = await controller.Create(createDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(ObjectController.GetById), createdResult.ActionName);
        var obj = Assert.IsType<ObjectDto>(createdResult.Value);
        Assert.Equal(1, obj.ObjectID);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenExceptionIsThrown()
    {
        // Arrange
        var mockService = new Mock<IObjectService>();
        var createDto = new CreateObjectDto { ObjectCode = "OBJ-001" };
        mockService.Setup(s => s.CreateAsync(It.IsAny<CreateObjectDto>()))
            .ThrowsAsync(new Exception("ObjectCode bestaat al"));
        var controller = new ObjectController(mockService.Object);

        // Act
        var result = await controller.Create(createDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("ObjectCode bestaat al", badRequestResult.Value);
    }

    [Fact]
    public async Task Create_CallsServiceCreate_WithCorrectDto()
    {
        // Arrange
        var mockService = new Mock<IObjectService>();
        var createDto = new CreateObjectDto { ObjectCode = "OBJ-001", Name = "Test" };
        mockService.Setup(s => s.CreateAsync(It.IsAny<CreateObjectDto>()))
            .ReturnsAsync(new ObjectDto { ObjectID = 1 });
        var controller = new ObjectController(mockService.Object);

        // Act
        await controller.Create(createDto);

        // Assert
        mockService.Verify(s => s.CreateAsync(
            It.Is<CreateObjectDto>(dto => dto.ObjectCode == "OBJ-001" && dto.Name == "Test")
        ), Times.Once);
    }

    #endregion

    #region PUT Tests

    [Fact]
    public async Task Update_ReturnsOkResult_WhenSuccessful()
    {
        // Arrange
        var mockService = new Mock<IObjectService>();
        var updateDto = new UpdateObjectDto
        {
            Name = "Geüpdatet Object",
            Description = "Nieuwe beschrijving",
            Location = "Nieuwe locatie",
            ChangedBy = "TestUser"
        };

        var updatedObject = new ObjectDto
        {
            ObjectID = 1,
            Name = "Geüpdatet Object",
            Description = "Nieuwe beschrijving"
        };

        mockService.Setup(s => s.UpdateAsync(1, It.IsAny<UpdateObjectDto>()))
            .ReturnsAsync(updatedObject);
        var controller = new ObjectController(mockService.Object);

        // Act
        var result = await controller.Update(1, updateDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var obj = Assert.IsType<ObjectDto>(okResult.Value);
        Assert.Equal("Geüpdatet Object", obj.Name);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenExceptionIsThrown()
    {
        // Arrange
        var mockService = new Mock<IObjectService>();
        var updateDto = new UpdateObjectDto { Name = "Test" };
        mockService.Setup(s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<UpdateObjectDto>()))
            .ThrowsAsync(new Exception("Object niet gevonden"));
        var controller = new ObjectController(mockService.Object);

        // Act
        var result = await controller.Update(1, updateDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Object niet gevonden", badRequestResult.Value);
    }

    [Fact]
    public async Task Update_CallsServiceUpdate_WithCorrectParameters()
    {
        // Arrange
        var mockService = new Mock<IObjectService>();
        var updateDto = new UpdateObjectDto { Name = "Test Update" };
        mockService.Setup(s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<UpdateObjectDto>()))
            .ReturnsAsync(new ObjectDto { ObjectID = 7 });
        var controller = new ObjectController(mockService.Object);

        // Act
        await controller.Update(7, updateDto);

        // Assert
        mockService.Verify(s => s.UpdateAsync(7, updateDto), Times.Once);
    }

    #endregion

    #region DELETE Tests

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenSuccessful()
    {
        // Arrange
        var mockService = new Mock<IObjectService>();
        mockService.Setup(s => s.DeleteAsync(It.IsAny<int>())).Returns(Task.CompletedTask);
        var controller = new ObjectController(mockService.Object);

        // Act
        var result = await controller.Delete(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsBadRequest_WhenExceptionIsThrown()
    {
        // Arrange
        var mockService = new Mock<IObjectService>();
        mockService.Setup(s => s.DeleteAsync(It.IsAny<int>()))
            .ThrowsAsync(new Exception("Kan niet verwijderen - heeft nog tickets"));
        var controller = new ObjectController(mockService.Object);

        // Act
        var result = await controller.Delete(1);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Kan niet verwijderen - heeft nog tickets", badRequestResult.Value);
    }

    [Fact]
    public async Task Delete_CallsServiceDelete_WithCorrectId()
    {
        // Arrange
        var mockService = new Mock<IObjectService>();
        mockService.Setup(s => s.DeleteAsync(It.IsAny<int>())).Returns(Task.CompletedTask);
        var controller = new ObjectController(mockService.Object);

        // Act
        await controller.Delete(15);

        // Assert
        mockService.Verify(s => s.DeleteAsync(15), Times.Once);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(999)]
    public async Task Delete_WorksForDifferentIds(int id)
    {
        // Arrange
        var mockService = new Mock<IObjectService>();
        mockService.Setup(s => s.DeleteAsync(It.IsAny<int>())).Returns(Task.CompletedTask);
        var controller = new ObjectController(mockService.Object);

        // Act
        var result = await controller.Delete(id);

        // Assert
        Assert.IsType<NoContentResult>(result);
        mockService.Verify(s => s.DeleteAsync(id), Times.Once);
    }

    #endregion
}