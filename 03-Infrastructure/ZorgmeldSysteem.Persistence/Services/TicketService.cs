using ZorgmeldSysteem.Application.Interfaces.IServices;
using ZorgmeldSysteem.Application.DTOs.Ticket;
using ZorgmeldSysteem.Domain.Entities;
using ZorgmeldSysteem.Domain.Enums;
using ZorgmeldSysteem.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ZorgmeldSysteem.Application.Services;

public class TicketService : ITicketService
{
    private readonly ZorgmeldContext _context;

    public TicketService(ZorgmeldContext context)
    {
        _context = context;
    }

    // Haal één ticket op
    public async Task<TicketDto?> GetByIdAsync(int id)
    {
        Ticket? ticket = await _context.Tickets
            .Include(t => t.Company)
            .Include(t => t.Mechanic)
            .Include(t => t.Object)
            .FirstOrDefaultAsync(t => t.TicketID == id);

        if (ticket == null)
        {
            return null;
        }

        // Map naar DTO
        return MapToDto(ticket);
    }

    // Zoek op ticketcode
    public async Task<TicketDto?> GetByTicketCodeAsync(string ticketCode)
    {
        Ticket? ticket = await _context.Tickets
            .Include(t => t.Company)
            .Include(t => t.Mechanic)
            .Include(t => t.Object)
            .FirstOrDefaultAsync(t => t.TicketCode == ticketCode);

        if (ticket == null)
        {
            return null;
        }

        return MapToDto(ticket);
    }

    // Haal alle tickets op
    public async Task<IEnumerable<TicketDto>> GetAllAsync()
    {
        List<Ticket> tickets = await _context.Tickets
            .Include(t => t.Company)
            .Include(t => t.Mechanic)
            .Include(t => t.Object)
            .ToListAsync();

        List<TicketDto> result = new List<TicketDto>();
        foreach (Ticket ticket in tickets)
        {
            result.Add(MapToDto(ticket));
        }

        return result;
    }

    // Maak nieuwe ticket
    public async Task<TicketDto> CreateAsync(CreateTicketDto createDto)
    {
        // Maak een nieuwe Ticket entity
        Ticket ticket = new Ticket
        {
            Description = createDto.Description,
            Note = createDto.Note ?? string.Empty,
            Location = createDto.Location ?? string.Empty,
            Priority = createDto.Priority,
            ReactionTime = createDto.ReactionTime,
            CompanyID = createDto.CompanyID,
            MechanicID = createDto.MechanicID,
            ObjectId = createDto.ObjectId,
            CreatedBy = createDto.CreatedBy,
            CreatedOn = DateTime.Now,
            Status = TicketStatus.Open,
            IsUrgent = createDto.IsUrgent,
            TicketCode = await GenerateTicketCodeAsync() 
        };

        // Voeg toe aan database
        await _context.Tickets.AddAsync(ticket);
        await _context.SaveChangesAsync();

        // Haal op met includes (voor complete DTO)
        Ticket? savedTicket = await _context.Tickets
            .Include(t => t.Company)
            .Include(t => t.Mechanic)
            .Include(t => t.Object)
            .FirstOrDefaultAsync(t => t.TicketID == ticket.TicketID);

        return MapToDto(savedTicket!);
    }

    // Update ticket
    public async Task<TicketDto> UpdateAsync(int id, UpdateTicketDto updateDto)
    {
        Ticket? ticket = await _context.Tickets.FindAsync(id);

        if (ticket == null)
        {
            throw new Exception($"Ticket met id {id} niet gevonden");
        }

        // Update alleen velden die zijn meegegeven
        if (updateDto.Description != null)
            ticket.Description = updateDto.Description;

        if (updateDto.Note != null)
            ticket.Note = updateDto.Note;

        if (updateDto.Location != null)
            ticket.Location = updateDto.Location;

        if (updateDto.Status.HasValue)
            ticket.Status = updateDto.Status.Value;

        if (updateDto.Priority.HasValue)
            ticket.Priority = updateDto.Priority.Value;

        if (updateDto.ReactionTime.HasValue)
            ticket.ReactionTime = updateDto.ReactionTime.Value;

        if (updateDto.MechanicID.HasValue)
            ticket.MechanicID = updateDto.MechanicID.Value;

        if (updateDto.ScheduledOn.HasValue)
            ticket.ScheduledOn = updateDto.ScheduledOn;

        if (updateDto.Solution != null)
            ticket.Solution = updateDto.Solution;

        ticket.IsUrgent = updateDto.IsUrgent;

        ticket.ChangedBy = updateDto.ChangedBy;
        ticket.ChangedOn = DateTime.Now;

        if (updateDto.Status == TicketStatus.Solved)
        {
            ticket.SolvedOn = DateTime.Now;
        }

        _context.Tickets.Update(ticket);
        await _context.SaveChangesAsync();

        Ticket? updatedTicket = await _context.Tickets
            .Include(t => t.Company)
            .Include(t => t.Mechanic)
            .Include(t => t.Object)
            .FirstOrDefaultAsync(t => t.TicketID == id);

        return MapToDto(updatedTicket!);
    }

    // Verwijder ticket
    public async Task DeleteAsync(int id)
    {
        Ticket? ticket = await _context.Tickets.FindAsync(id);

        if (ticket == null)
        {
            throw new Exception($"Ticket met id {id} niet gevonden");
        }

        _context.Tickets.Remove(ticket);
        await _context.SaveChangesAsync();
    }

    // Haal tickets op per bedrijf
    public async Task<IEnumerable<TicketDto>> GetByCompanyIdAsync(int companyId)
    {
        List<Ticket> tickets = await _context.Tickets
            .Include(t => t.Company)
            .Include(t => t.Mechanic)
            .Include(t => t.Object)
            .ToListAsync();

        List<TicketDto> result = new List<TicketDto>();
        foreach (Ticket ticket in tickets)
        {
            if (ticket.CompanyID == companyId)
            {
                result.Add(MapToDto(ticket));
            }
        }

        return result;
    }

    // Haal tickets op per monteur
    public async Task<IEnumerable<TicketDto>> GetByMechanicIdAsync(int mechanicId)
    {
        List<Ticket> tickets = await _context.Tickets
            .Include(t => t.Company)
            .Include(t => t.Mechanic)
            .Include(t => t.Object)
            .ToListAsync();

        List<TicketDto> result = new List<TicketDto>();
        foreach (Ticket ticket in tickets)
        {
            if (ticket.MechanicID == mechanicId)
            {
                result.Add(MapToDto(ticket));
            }
        }

        return result;
    }

    // Haal tickets op per status
    public async Task<IEnumerable<TicketDto>> GetByStatusAsync(TicketStatus status)
    {
        List<Ticket> tickets = await _context.Tickets
            .Include(t => t.Company)
            .Include(t => t.Mechanic)
            .Include(t => t.Object)
            .ToListAsync();

        List<TicketDto> result = new List<TicketDto>();
        foreach (Ticket ticket in tickets)
        {
            if (ticket.Status == status)
            {
                result.Add(MapToDto(ticket));
            }
        }

        return result;
    }

    // Haal open tickets op
    public async Task<IEnumerable<TicketDto>> GetOpenTicketsAsync()
    {
        List<Ticket> tickets = await _context.Tickets
            .Include(t => t.Company)
            .Include(t => t.Mechanic)
            .Include(t => t.Object)
            .ToListAsync();

        List<TicketDto> result = new List<TicketDto>();
        foreach (Ticket ticket in tickets)
        {
            if (ticket.Status == TicketStatus.Open || ticket.Status == TicketStatus.Assigned)
            {
                result.Add(MapToDto(ticket));
            }
        }

        return result;
    }

    // Wijs monteur toe
    public async Task<bool> AssignMechanicAsync(int ticketId, int mechanicId)
    {
        Ticket? ticket = await _context.Tickets.FindAsync(ticketId);

        if (ticket == null)
        {
            return false;
        }

        ticket.MechanicID = mechanicId;
        ticket.Status = TicketStatus.Assigned;
        ticket.ChangedOn = DateTime.Now;

        _context.Tickets.Update(ticket);
        await _context.SaveChangesAsync();

        return true;
    }

    // Update status
    public async Task<bool> UpdateStatusAsync(int ticketId, TicketStatus newStatus)
    {
        Ticket? ticket = await _context.Tickets.FindAsync(ticketId);

        if (ticket == null)
        {
            return false;
        }

        ticket.Status = newStatus;
        ticket.ChangedOn = DateTime.Now;

        if (newStatus == TicketStatus.Solved)
        {
            ticket.SolvedOn = DateTime.Now;
        }

        _context.Tickets.Update(ticket);
        await _context.SaveChangesAsync();

        return true;
    }

    // PRIVATE HELPER METHODS

    // Genereer unieke ticketcode
    private async Task<string> GenerateTicketCodeAsync()
    {
        int year = DateTime.Now.Year;
        int totalTickets = await _context.Tickets.CountAsync();
        int nextNumber = totalTickets + 1;

        return $"TK-{year}-{nextNumber:D4}";
    }

    // Map Entity naar DTO
    private TicketDto MapToDto(Ticket ticket)
    {
        return new TicketDto
        {
            TicketID = ticket.TicketID,
            TicketCode = ticket.TicketCode,
            Description = ticket.Description,
            Note = ticket.Note,
            Location = ticket.Location,
            Status = ticket.Status,
            Priority = ticket.Priority,
            ReactionTime = ticket.ReactionTime,
            IsUrgent = ticket.IsUrgent,
            CreatedOn = ticket.CreatedOn,
            LeadTime = ticket.LeadTime,
            ScheduledOn = ticket.ScheduledOn,
            SolvedOn = ticket.SolvedOn,
            Solution = ticket.Solution,
            CompanyID = ticket.CompanyID,
            CompanyName = ticket.Company?.Name ?? string.Empty,
            MechanicID = ticket.MechanicID,
            MechanicName = ticket.Mechanic?.Name,
            ObjectId = ticket.ObjectId,
            ObjectName = ticket.Object?.Name
        };
    }
}