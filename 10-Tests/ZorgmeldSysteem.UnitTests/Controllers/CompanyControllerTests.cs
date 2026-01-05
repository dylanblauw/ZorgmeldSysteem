using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using ZorgmeldSysteem.Application.DTOs.Company;
using ZorgmeldSysteem.Application.Interfaces.IServices;
using ZorgmeldSysteem.WebApi.Controllers;

namespace ZorgmeldSysteem.UnitTests.Controllers;

/// <summary>
/// Unit Tests voor CompanyController
/// Test alle endpoints en scenario's (success, not found, exceptions)
/// </summary>
public class CompanyControllerTests
{
    #region GET Tests

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithListOfCompanies()
    {
        // Arrange - Voorbereiden
        var mockService = new Mock<ICompanyService>();
        var testCompanies = new List<CompanyDto>
        {
            new CompanyDto { CompanyID = 1, Name = "Bedrijf A", Email = "a@test.nl" },
            new CompanyDto { CompanyID = 2, Name = "Bedrijf B", Email = "b@test.nl" }
        };
        mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(testCompanies);
        var controller = new CompanyController(mockService.Object);

        // Act - Uitvoeren
        var result = await controller.GetAll();

        // Assert - Controleren
        var okResult = Assert.IsType<OkObjectResult>(result);
        var companies = Assert.IsAssignableFrom<IEnumerable<CompanyDto>>(okResult.Value);
        Assert.Equal(2, companies.Count());
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithEmptyList_WhenNoCompaniesExist()
    {
        // Arrange
        var mockService = new Mock<ICompanyService>();
        mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<CompanyDto>());
        var controller = new CompanyController(mockService.Object);

        // Act
        var result = await controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var companies = Assert.IsAssignableFrom<IEnumerable<CompanyDto>>(okResult.Value);
        Assert.Empty(companies);
    }

    [Fact]
    public async Task GetById_ReturnsOkResult_WhenCompanyExists()
    {
        // Arrange
        var mockService = new Mock<ICompanyService>();
        var testCompany = new CompanyDto
        {
            CompanyID = 1,
            Name = "Test BV",
            Email = "test@test.nl",
            Phonenumber = "0612345678",
            City = "Amsterdam"
        };
        mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(testCompany);
        var controller = new CompanyController(mockService.Object);

        // Act
        var result = await controller.GetById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var company = Assert.IsType<CompanyDto>(okResult.Value);
        Assert.Equal(1, company.CompanyID);
        Assert.Equal("Test BV", company.Name);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenCompanyDoesNotExist()
    {
        // Arrange
        var mockService = new Mock<ICompanyService>();
        mockService.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((CompanyDto?)null);
        var controller = new CompanyController(mockService.Object);

        // Act
        var result = await controller.GetById(999);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Bedrijf met id 999 niet gevonden", notFoundResult.Value);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(100)]
    public async Task GetById_CallsServiceWithCorrectId(int testId)
    {
        // Arrange
        var mockService = new Mock<ICompanyService>();
        mockService.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((CompanyDto?)null);
        var controller = new CompanyController(mockService.Object);

        // Act
        await controller.GetById(testId);

        // Assert - Verify dat de service method werd aangeroepen met de juiste ID
        mockService.Verify(s => s.GetByIdAsync(testId), Times.Once);
    }

    [Fact]
    public async Task GetExternal_ReturnsOkResult_WithExternalCompanies()
    {
        // Arrange
        var mockService = new Mock<ICompanyService>();
        var externalCompanies = new List<CompanyDto>
        {
            new CompanyDto { CompanyID = 1, Name = "Extern A", IsExternal = true },
            new CompanyDto { CompanyID = 2, Name = "Extern B", IsExternal = true }
        };
        mockService.Setup(s => s.GetExternalCompaniesAsync()).ReturnsAsync(externalCompanies);
        var controller = new CompanyController(mockService.Object);

        // Act
        var result = await controller.GetExternal();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var companies = Assert.IsAssignableFrom<IEnumerable<CompanyDto>>(okResult.Value);
        Assert.All(companies, c => Assert.True(c.IsExternal));
    }

    #endregion

    #region POST Tests

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WhenSuccessful()
    {
        // Arrange
        var mockService = new Mock<ICompanyService>();
        var createDto = new CreateCompanyDto
        {
            Name = "Nieuw Bedrijf",
            Email = "nieuw@test.nl",
            Phonenumber = "0612345678",
            Street = "Teststraat",
            HouseNumber = "1",
            PostalCode = "1234AB",
            City = "Amsterdam",
            Province = "Noord-Holland",
            Country = "Nederland",
            Contact = "Jan Jansen",
            IsExternal = false,
            CreatedBy = "TestUser"
        };

        var createdCompany = new CompanyDto
        {
            CompanyID = 1,
            Name = createDto.Name,
            Email = createDto.Email,
            Phonenumber = createDto.Phonenumber
        };

        mockService.Setup(s => s.CreateAsync(It.IsAny<CreateCompanyDto>()))
            .ReturnsAsync(createdCompany);
        var controller = new CompanyController(mockService.Object);

        // Act
        var result = await controller.Create(createDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(CompanyController.GetById), createdResult.ActionName);
        Assert.Equal(1, ((CompanyDto)createdResult.Value!).CompanyID);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenExceptionIsThrown()
    {
        // Arrange
        var mockService = new Mock<ICompanyService>();
        var createDto = new CreateCompanyDto { Name = "Test" };
        mockService.Setup(s => s.CreateAsync(It.IsAny<CreateCompanyDto>()))
            .ThrowsAsync(new Exception("Database fout"));
        var controller = new CompanyController(mockService.Object);

        // Act
        var result = await controller.Create(createDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Database fout", badRequestResult.Value);
    }

    [Fact]
    public async Task Create_CallsServiceCreate_WithCorrectDto()
    {
        // Arrange
        var mockService = new Mock<ICompanyService>();
        var createDto = new CreateCompanyDto { Name = "Test", Email = "test@test.nl" };
        mockService.Setup(s => s.CreateAsync(It.IsAny<CreateCompanyDto>()))
            .ReturnsAsync(new CompanyDto { CompanyID = 1 });
        var controller = new CompanyController(mockService.Object);

        // Act
        await controller.Create(createDto);

        // Assert
        mockService.Verify(s => s.CreateAsync(
            It.Is<CreateCompanyDto>(dto => dto.Name == "Test" && dto.Email == "test@test.nl")
        ), Times.Once);
    }

    #endregion

    #region PUT Tests

    [Fact]
    public async Task Update_ReturnsOkResult_WhenSuccessful()
    {
        // Arrange
        var mockService = new Mock<ICompanyService>();
        var updateDto = new UpdateCompanyDto
        {
            Name = "Geüpdatet Bedrijf",
            Email = "updated@test.nl",
            ChangedBy = "TestUser"
        };

        var updatedCompany = new CompanyDto
        {
            CompanyID = 1,
            Name = "Geüpdatet Bedrijf",
            Email = "updated@test.nl"
        };

        mockService.Setup(s => s.UpdateAsync(1, It.IsAny<UpdateCompanyDto>()))
            .ReturnsAsync(updatedCompany);
        var controller = new CompanyController(mockService.Object);

        // Act
        var result = await controller.Update(1, updateDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var company = Assert.IsType<CompanyDto>(okResult.Value);
        Assert.Equal("Geüpdatet Bedrijf", company.Name);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenExceptionIsThrown()
    {
        // Arrange
        var mockService = new Mock<ICompanyService>();
        var updateDto = new UpdateCompanyDto { Name = "Test" };
        mockService.Setup(s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<UpdateCompanyDto>()))
            .ThrowsAsync(new Exception("Update mislukt"));
        var controller = new CompanyController(mockService.Object);

        // Act
        var result = await controller.Update(1, updateDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Update mislukt", badRequestResult.Value);
    }

    [Fact]
    public async Task Update_CallsServiceUpdate_WithCorrectParameters()
    {
        // Arrange
        var mockService = new Mock<ICompanyService>();
        var updateDto = new UpdateCompanyDto { Name = "Test Update" };
        mockService.Setup(s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<UpdateCompanyDto>()))
            .ReturnsAsync(new CompanyDto { CompanyID = 5 });
        var controller = new CompanyController(mockService.Object);

        // Act
        await controller.Update(5, updateDto);

        // Assert
        mockService.Verify(s => s.UpdateAsync(5, updateDto), Times.Once);
    }

    #endregion

    #region DELETE Tests

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenSuccessful()
    {
        // Arrange
        var mockService = new Mock<ICompanyService>();
        mockService.Setup(s => s.DeleteAsync(It.IsAny<int>())).Returns(Task.CompletedTask);
        var controller = new CompanyController(mockService.Object);

        // Act
        var result = await controller.Delete(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsBadRequest_WhenExceptionIsThrown()
    {
        // Arrange
        var mockService = new Mock<ICompanyService>();
        mockService.Setup(s => s.DeleteAsync(It.IsAny<int>()))
            .ThrowsAsync(new Exception("Kan niet verwijderen - heeft nog tickets"));
        var controller = new CompanyController(mockService.Object);

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
        var mockService = new Mock<ICompanyService>();
        mockService.Setup(s => s.DeleteAsync(It.IsAny<int>())).Returns(Task.CompletedTask);
        var controller = new CompanyController(mockService.Object);

        // Act
        await controller.Delete(42);

        // Assert
        mockService.Verify(s => s.DeleteAsync(42), Times.Once);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(999)]
    public async Task Delete_WorksForDifferentIds(int id)
    {
        // Arrange
        var mockService = new Mock<ICompanyService>();
        mockService.Setup(s => s.DeleteAsync(It.IsAny<int>())).Returns(Task.CompletedTask);
        var controller = new CompanyController(mockService.Object);

        // Act
        var result = await controller.Delete(id);

        // Assert
        Assert.IsType<NoContentResult>(result);
        mockService.Verify(s => s.DeleteAsync(id), Times.Once);
    }

    #endregion
}