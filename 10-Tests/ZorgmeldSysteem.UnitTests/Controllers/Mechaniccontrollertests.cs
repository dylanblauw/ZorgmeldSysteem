using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using ZorgmeldSysteem.Application.DTOs.Mechanic;
using ZorgmeldSysteem.Application.Interfaces.IServices;
using ZorgmeldSysteem.Domain.Enums;
using ZorgmeldSysteem.WebApi.Controllers;

namespace ZorgmeldSysteem.UnitTests.Controllers;

/// <summary>
/// Unit Tests voor MechanicController
/// Test alle endpoints voor monteur beheer
/// </summary>
public class MechanicControllerTests
{
    #region GET Tests

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithListOfMechanics()
    {
        // Arrange
        var mockService = new Mock<IMechanicService>();
        var testMechanics = new List<MechanicDto>
        {
            new MechanicDto { MechanicID = 1, Name = "Jan Jansen", Email = "jan@test.nl" },
            new MechanicDto { MechanicID = 2, Name = "Piet Pietersen", Email = "piet@test.nl" }
        };
        mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(testMechanics);
        var controller = new MechanicController(mockService.Object);

        // Act
        var result = await controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var mechanics = Assert.IsAssignableFrom<IEnumerable<MechanicDto>>(okResult.Value);
        Assert.Equal(2, mechanics.Count());
    }

    [Fact]
    public async Task GetById_ReturnsOkResult_WhenMechanicExists()
    {
        // Arrange
        var mockService = new Mock<IMechanicService>();
        var testMechanic = new MechanicDto
        {
            MechanicID = 1,
            Name = "Jan Jansen",
            Email = "jan@test.nl",
            Phonenumber = "0612345678",
            Type = MechanicType.InternalGeneral,
            IsActive = true
        };
        mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(testMechanic);
        var controller = new MechanicController(mockService.Object);

        // Act
        var result = await controller.GetById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var mechanic = Assert.IsType<MechanicDto>(okResult.Value);
        Assert.Equal(1, mechanic.MechanicID);
        Assert.Equal("Jan Jansen", mechanic.Name);
    }

    [Fact]
    public async Task GetActive_ReturnsOkResult_WithOnlyActiveMechanics()
    {
        // Arrange
        var mockService = new Mock<IMechanicService>();
        var activeMechanics = new List<MechanicDto>
        {
            new MechanicDto { MechanicID = 1, Name = "Jan", IsActive = true },
            new MechanicDto { MechanicID = 2, Name = "Piet", IsActive = true }
        };
        mockService.Setup(s => s.GetActiveAsync()).ReturnsAsync(activeMechanics);
        var controller = new MechanicController(mockService.Object);

        // Act
        var result = await controller.GetActive();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var mechanics = Assert.IsAssignableFrom<IEnumerable<MechanicDto>>(okResult.Value);
        Assert.All(mechanics, m => Assert.True(m.IsActive));
    }

    [Theory]
    [InlineData(MechanicType.InternalGeneral)]
    [InlineData(MechanicType.InternalElectrical)]
    [InlineData(MechanicType.ExternalIT)]
    public async Task GetByType_ReturnsOkResult_WithMechanicsOfSpecifiedType(MechanicType type)
    {
        // Arrange
        var mockService = new Mock<IMechanicService>();
        var mechanicsOfType = new List<MechanicDto>
        {
            new MechanicDto { MechanicID = 1, Type = type },
            new MechanicDto { MechanicID = 2, Type = type }
        };
        mockService.Setup(s => s.GetByTypeAsync(type)).ReturnsAsync(mechanicsOfType);
        var controller = new MechanicController(mockService.Object);

        // Act
        var result = await controller.GetByType(type);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var mechanics = Assert.IsAssignableFrom<IEnumerable<MechanicDto>>(okResult.Value);
        Assert.All(mechanics, m => Assert.Equal(type, m.Type));
    }

    #endregion

    #region POST Tests

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WhenSuccessful()
    {
        // Arrange
        var mockService = new Mock<IMechanicService>();
        var createDto = new CreateMechanicDto
        {
            Name = "Nieuwe Monteur",
            Email = "nieuw@test.nl",
            Phonenumber = "0612345678",
            Type = MechanicType.InternalGeneral,
            CompanyID = 1,
            TempPassword = "Welkom123!"
        };

        var createdMechanic = new MechanicDto
        {
            MechanicID = 1,
            Name = createDto.Name,
            Email = createDto.Email,
            Type = createDto.Type,
            IsActive = true
        };

        mockService.Setup(s => s.CreateAsync(It.IsAny<CreateMechanicDto>()))
            .ReturnsAsync(createdMechanic);
        var controller = new MechanicController(mockService.Object);

        // Act
        var result = await controller.Create(createDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(MechanicController.GetById), createdResult.ActionName);
        var mechanic = Assert.IsType<MechanicDto>(createdResult.Value);
        Assert.Equal(1, mechanic.MechanicID);
    }

    [Fact]
    public async Task Create_ReturnsInternalServerError_WhenUnexpectedExceptionIsThrown()
    {
        // Arrange
        var mockService = new Mock<IMechanicService>();
        var createDto = new CreateMechanicDto { Email = "test@test.nl" };
        mockService.Setup(s => s.CreateAsync(It.IsAny<CreateMechanicDto>()))
            .ThrowsAsync(new Exception("Database fout"));
        var controller = new MechanicController(mockService.Object);

        // Act
        var result = await controller.Create(createDto);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objectResult.StatusCode);
    }

    #endregion

    #region PUT Tests

    [Fact]
    public async Task Update_ReturnsOkResult_WhenSuccessful()
    {
        // Arrange
        var mockService = new Mock<IMechanicService>();
        var updateDto = new UpdateMechanicDto
        {
            Name = "Geüpdate Naam",
            Email = "updated@test.nl",
            IsActive = false,
            ChangedBy = "TestUser"
        };

        var updatedMechanic = new MechanicDto
        {
            MechanicID = 1,
            Name = "Geüpdate Naam",
            Email = "updated@test.nl",
            IsActive = false
        };

        mockService.Setup(s => s.UpdateAsync(1, It.IsAny<UpdateMechanicDto>()))
            .ReturnsAsync(updatedMechanic);
        var controller = new MechanicController(mockService.Object);

        // Act
        var result = await controller.Update(1, updateDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var mechanic = Assert.IsType<MechanicDto>(okResult.Value);
        Assert.Equal("Geüpdate Naam", mechanic.Name);
        Assert.False(mechanic.IsActive);
    }

    [Fact]
    public async Task Update_ReturnsInternalServerError_WhenUnexpectedExceptionIsThrown()
    {
        // Arrange
        var mockService = new Mock<IMechanicService>();
        var updateDto = new UpdateMechanicDto();
        mockService.Setup(s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<UpdateMechanicDto>()))
            .ThrowsAsync(new Exception("Database fout"));
        var controller = new MechanicController(mockService.Object);

        // Act
        var result = await controller.Update(1, updateDto);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objectResult.StatusCode);
    }

    #endregion

    #region DELETE Tests

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenSuccessful()
    {
        // Arrange
        var mockService = new Mock<IMechanicService>();
        mockService.Setup(s => s.DeleteAsync(It.IsAny<int>())).Returns(Task.CompletedTask);
        var controller = new MechanicController(mockService.Object);

        // Act
        var result = await controller.Delete(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    
    [Fact]
    public async Task Delete_ReturnsInternalServerError_WhenUnexpectedExceptionIsThrown()
    {
        // Arrange
        var mockService = new Mock<IMechanicService>();
        mockService.Setup(s => s.DeleteAsync(It.IsAny<int>()))
            .ThrowsAsync(new Exception("Kan niet verwijderen - heeft nog tickets"));
        var controller = new MechanicController(mockService.Object);

        // Act
        var result = await controller.Delete(1);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objectResult.StatusCode);
    }

    [Fact]
    public async Task Delete_CallsServiceDelete_WithCorrectId()
    {
        // Arrange
        var mockService = new Mock<IMechanicService>();
        mockService.Setup(s => s.DeleteAsync(It.IsAny<int>())).Returns(Task.CompletedTask);
        var controller = new MechanicController(mockService.Object);

        // Act
        await controller.Delete(42);

        // Assert
        mockService.Verify(s => s.DeleteAsync(42), Times.Once);
    }

    #endregion
}