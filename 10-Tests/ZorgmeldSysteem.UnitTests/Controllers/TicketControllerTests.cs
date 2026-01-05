using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using ZorgmeldSysteem.Application.DTOs.Ticket;
using ZorgmeldSysteem.Application.Interfaces.IServices;
using ZorgmeldSysteem.Domain.Enums;
using ZorgmeldSysteem.WebApi.Controllers;

namespace ZorgmeldSysteem.UnitTests.Controllers;

/// <summary>
/// Unit Tests voor TicketController
/// Test alle endpoints inclusief specifieke ticket operations
/// </summary>
public class TicketControllerTests
{
    #region GET Tests - Basic

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithListOfTickets()
    {
        // Arrange
        var mockService = new Mock<ITicketService>();
        var testTickets = new List<TicketDto>
        {
            new TicketDto { TicketID = 1, TicketCode = "TK-2025-0001", Description = "Ticket 1" },
            new TicketDto { TicketID = 2, TicketCode = "TK-2025-0002", Description = "Ticket 2" }
        };
        mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(testTickets);
        var controller = new TicketController(mockService.Object);

        // Act
        var result = await controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var tickets = Assert.IsAssignableFrom<IEnumerable<TicketDto>>(okResult.Value);
        Assert.Equal(2, tickets.Count());
    }

    [Fact]
    public async Task GetById_ReturnsOkResult_WhenTicketExists()
    {
        // Arrange
        var mockService = new Mock<ITicketService>();
        var testTicket = new TicketDto
        {
            TicketID = 1,
            TicketCode = "TK-2025-0001",
            Description = "Test ticket",
            Status = TicketStatus.Open,
            Priority = Priority.High
        };
        mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(testTicket);
        var controller = new TicketController(mockService.Object);

        // Act
        var result = await controller.GetById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var ticket = Assert.IsType<TicketDto>(okResult.Value);
        Assert.Equal(1, ticket.TicketID);
        Assert.Equal("TK-2025-0001", ticket.TicketCode);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenTicketDoesNotExist()
    {
        // Arrange
        var mockService = new Mock<ITicketService>();
        mockService.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((TicketDto?)null);
        var controller = new TicketController(mockService.Object);

        // Act
        var result = await controller.GetById(999);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Ticket met id 999 niet gevonden", notFoundResult.Value);
    }

    #endregion

    #region GET Tests - By TicketCode

    [Fact]
    public async Task GetByTicketCode_ReturnsOkResult_WhenTicketExists()
    {
        // Arrange
        var mockService = new Mock<ITicketService>();
        var testTicket = new TicketDto
        {
            TicketID = 1,
            TicketCode = "TK-2025-0001",
            Description = "Test ticket"
        };
        mockService.Setup(s => s.GetByTicketCodeAsync("TK-2025-0001")).ReturnsAsync(testTicket);
        var controller = new TicketController(mockService.Object);

        // Act
        var result = await controller.GetByTicketCode("TK-2025-0001");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var ticket = Assert.IsType<TicketDto>(okResult.Value);
        Assert.Equal("TK-2025-0001", ticket.TicketCode);
    }

    [Fact]
    public async Task GetByTicketCode_ReturnsNotFound_WhenTicketDoesNotExist()
    {
        // Arrange
        var mockService = new Mock<ITicketService>();
        mockService.Setup(s => s.GetByTicketCodeAsync("TK-9999-9999")).ReturnsAsync((TicketDto?)null);
        var controller = new TicketController(mockService.Object);

        // Act
        var result = await controller.GetByTicketCode("TK-9999-9999");

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Ticket met code TK-9999-9999 niet gevonden", notFoundResult.Value);
    }

    #endregion

    #region GET Tests - By CompanyId

    [Fact]
    public async Task GetByCompanyId_ReturnsOkResult_WithTicketsForCompany()
    {
        // Arrange
        var mockService = new Mock<ITicketService>();
        var companyTickets = new List<TicketDto>
        {
            new TicketDto { TicketID = 1, CompanyID = 5, Description = "Ticket 1" },
            new TicketDto { TicketID = 2, CompanyID = 5, Description = "Ticket 2" }
        };
        mockService.Setup(s => s.GetByCompanyIdAsync(5)).ReturnsAsync(companyTickets);
        var controller = new TicketController(mockService.Object);

        // Act
        var result = await controller.GetByCompanyId(5);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var tickets = Assert.IsAssignableFrom<IEnumerable<TicketDto>>(okResult.Value);
        Assert.Equal(2, tickets.Count());
        Assert.All(tickets, t => Assert.Equal(5, t.CompanyID));
    }

    [Fact]
    public async Task GetByCompanyId_ReturnsEmptyList_WhenNoTicketsExist()
    {
        // Arrange
        var mockService = new Mock<ITicketService>();
        mockService.Setup(s => s.GetByCompanyIdAsync(99)).ReturnsAsync(new List<TicketDto>());
        var controller = new TicketController(mockService.Object);

        // Act
        var result = await controller.GetByCompanyId(99);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var tickets = Assert.IsAssignableFrom<IEnumerable<TicketDto>>(okResult.Value);
        Assert.Empty(tickets);
    }

    #endregion

    #region GET Tests - By MechanicId

    [Fact]
    public async Task GetByMechanicId_ReturnsOkResult_WithTicketsForMechanic()
    {
        // Arrange
        var mockService = new Mock<ITicketService>();
        var mechanicTickets = new List<TicketDto>
        {
            new TicketDto { TicketID = 1, MechanicID = 3, Description = "Ticket 1" },
            new TicketDto { TicketID = 2, MechanicID = 3, Description = "Ticket 2" }
        };
        mockService.Setup(s => s.GetByMechanicIdAsync(3)).ReturnsAsync(mechanicTickets);
        var controller = new TicketController(mockService.Object);

        // Act
        var result = await controller.GetByMechanicId(3);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var tickets = Assert.IsAssignableFrom<IEnumerable<TicketDto>>(okResult.Value);
        Assert.All(tickets, t => Assert.Equal(3, t.MechanicID));
    }

    #endregion

    #region GET Tests - By Status

    [Theory]
    [InlineData(TicketStatus.Open)]
    [InlineData(TicketStatus.InProgress)]
    [InlineData(TicketStatus.Closed)]
    public async Task GetByStatus_ReturnsOkResult_WithTicketsOfSpecifiedStatus(TicketStatus status)
    {
        // Arrange
        var mockService = new Mock<ITicketService>();
        var ticketsWithStatus = new List<TicketDto>
        {
            new TicketDto { TicketID = 1, Status = status },
            new TicketDto { TicketID = 2, Status = status }
        };
        mockService.Setup(s => s.GetByStatusAsync(status)).ReturnsAsync(ticketsWithStatus);
        var controller = new TicketController(mockService.Object);

        // Act
        var result = await controller.GetByStatus(status);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var tickets = Assert.IsAssignableFrom<IEnumerable<TicketDto>>(okResult.Value);
        Assert.All(tickets, t => Assert.Equal(status, t.Status));
    }

    [Fact]
    public async Task GetOpen_ReturnsOkResult_WithOpenTickets()
    {
        // Arrange
        var mockService = new Mock<ITicketService>();
        var openTickets = new List<TicketDto>
        {
            new TicketDto { TicketID = 1, Status = TicketStatus.Open },
            new TicketDto { TicketID = 2, Status = TicketStatus.InProgress }
        };
        mockService.Setup(s => s.GetOpenTicketsAsync()).ReturnsAsync(openTickets);
        var controller = new TicketController(mockService.Object);

        // Act
        var result = await controller.GetOpen();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var tickets = Assert.IsAssignableFrom<IEnumerable<TicketDto>>(okResult.Value);
        Assert.Equal(2, tickets.Count());
    }

    #endregion

    #region POST Tests

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WhenSuccessful()
    {
        // Arrange
        var mockService = new Mock<ITicketService>();
        var createDto = new CreateTicketDto
        {
            Description = "Nieuwe storing",
            Location = "Kamer 101",
            Priority = Priority.High,
            ReactionTime = ReactionTime.Immediate,
            CompanyID = 1,
            CreatedBy = "TestUser"
        };

        var createdTicket = new TicketDto
        {
            TicketID = 1,
            TicketCode = "TK-2025-0001",
            Description = createDto.Description,
            Status = TicketStatus.Open
        };

        mockService.Setup(s => s.CreateAsync(It.IsAny<CreateTicketDto>()))
            .ReturnsAsync(createdTicket);
        var controller = new TicketController(mockService.Object);

        // Act
        var result = await controller.Create(createDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(TicketController.GetById), createdResult.ActionName);
        var ticket = Assert.IsType<TicketDto>(createdResult.Value);
        Assert.Equal(1, ticket.TicketID);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenExceptionIsThrown()
    {
        // Arrange
        var mockService = new Mock<ITicketService>();
        var createDto = new CreateTicketDto { Description = "Test" };
        mockService.Setup(s => s.CreateAsync(It.IsAny<CreateTicketDto>()))
            .ThrowsAsync(new Exception("Bedrijf niet gevonden"));
        var controller = new TicketController(mockService.Object);

        // Act
        var result = await controller.Create(createDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Bedrijf niet gevonden", badRequestResult.Value);
    }

    #endregion

    #region PUT Tests

    [Fact]
    public async Task Update_ReturnsOkResult_WhenSuccessful()
    {
        // Arrange
        var mockService = new Mock<ITicketService>();
        var updateDto = new UpdateTicketDto
        {
            Description = "Geüpdate beschrijving",
            Status = TicketStatus.InProgress,
            ChangedBy = "TestUser"
        };

        var updatedTicket = new TicketDto
        {
            TicketID = 1,
            Description = "Geüpdate beschrijving",
            Status = TicketStatus.InProgress
        };

        mockService.Setup(s => s.UpdateAsync(1, It.IsAny<UpdateTicketDto>()))
            .ReturnsAsync(updatedTicket);
        var controller = new TicketController(mockService.Object);

        // Act
        var result = await controller.Update(1, updateDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var ticket = Assert.IsType<TicketDto>(okResult.Value);
        Assert.Equal(TicketStatus.InProgress, ticket.Status);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenExceptionIsThrown()
    {
        // Arrange
        var mockService = new Mock<ITicketService>();
        var updateDto = new UpdateTicketDto();
        mockService.Setup(s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<UpdateTicketDto>()))
            .ThrowsAsync(new Exception("Ticket niet gevonden"));
        var controller = new TicketController(mockService.Object);

        // Act
        var result = await controller.Update(1, updateDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Ticket niet gevonden", badRequestResult.Value);
    }

    #endregion

    #region DELETE Tests

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenSuccessful()
    {
        // Arrange
        var mockService = new Mock<ITicketService>();
        mockService.Setup(s => s.DeleteAsync(It.IsAny<int>())).Returns(Task.CompletedTask);
        var controller = new TicketController(mockService.Object);

        // Act
        var result = await controller.Delete(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsBadRequest_WhenExceptionIsThrown()
    {
        // Arrange
        var mockService = new Mock<ITicketService>();
        mockService.Setup(s => s.DeleteAsync(It.IsAny<int>()))
            .ThrowsAsync(new Exception("Kan niet verwijderen"));
        var controller = new TicketController(mockService.Object);

        // Act
        var result = await controller.Delete(1);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Kan niet verwijderen", badRequestResult.Value);
    }

    #endregion

    #region PATCH Tests - Assign Mechanic

    [Fact]
    public async Task AssignMechanic_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var mockService = new Mock<ITicketService>();
        mockService.Setup(s => s.AssignMechanicAsync(1, 3)).ReturnsAsync(true);
        var controller = new TicketController(mockService.Object);

        // Act
        var result = await controller.AssignMechanic(1, 3);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Monteur succesvol toegewezen", okResult.Value);
    }

    [Fact]
    public async Task AssignMechanic_ReturnsNotFound_WhenTicketOrMechanicNotFound()
    {
        // Arrange
        var mockService = new Mock<ITicketService>();
        mockService.Setup(s => s.AssignMechanicAsync(999, 999)).ReturnsAsync(false);
        var controller = new TicketController(mockService.Object);

        // Act
        var result = await controller.AssignMechanic(999, 999);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Ticket of Monteur niet gevonden", notFoundResult.Value);
    }

    [Fact]
    public async Task AssignMechanic_CallsServiceWithCorrectParameters()
    {
        // Arrange
        var mockService = new Mock<ITicketService>();
        mockService.Setup(s => s.AssignMechanicAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(true);
        var controller = new TicketController(mockService.Object);

        // Act
        await controller.AssignMechanic(5, 10);

        // Assert
        mockService.Verify(s => s.AssignMechanicAsync(5, 10), Times.Once);
    }

    #endregion

    #region PATCH Tests - Update Status

    [Theory]
    [InlineData(TicketStatus.Open)]
    [InlineData(TicketStatus.InProgress)]
    [InlineData(TicketStatus.Closed)]
    [InlineData(TicketStatus.OnHold)]
    public async Task UpdateStatus_ReturnsOk_WhenSuccessful(TicketStatus newStatus)
    {
        // Arrange
        var mockService = new Mock<ITicketService>();
        mockService.Setup(s => s.UpdateStatusAsync(1, newStatus)).ReturnsAsync(true);
        var controller = new TicketController(mockService.Object);

        // Act
        var result = await controller.UpdateStatus(1, newStatus);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Status succesvol bijgewerkt", okResult.Value);
    }

    [Fact]
    public async Task UpdateStatus_ReturnsNotFound_WhenTicketNotFound()
    {
        // Arrange
        var mockService = new Mock<ITicketService>();
        mockService.Setup(s => s.UpdateStatusAsync(999, It.IsAny<TicketStatus>()))
            .ReturnsAsync(false);
        var controller = new TicketController(mockService.Object);

        // Act
        var result = await controller.UpdateStatus(999, TicketStatus.Closed);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Ticket met id 999 niet gevonden", notFoundResult.Value);
    }

    [Fact]
    public async Task UpdateStatus_CallsServiceWithCorrectParameters()
    {
        // Arrange
        var mockService = new Mock<ITicketService>();
        mockService.Setup(s => s.UpdateStatusAsync(It.IsAny<int>(), It.IsAny<TicketStatus>()))
            .ReturnsAsync(true);
        var controller = new TicketController(mockService.Object);

        // Act
        await controller.UpdateStatus(7, TicketStatus.Closed);

        // Assert
        mockService.Verify(s => s.UpdateStatusAsync(7, TicketStatus.Closed), Times.Once);
    }

    #endregion
}