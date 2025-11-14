using Microsoft.EntityFrameworkCore;
using ZorgmeldSysteem.Application.DTOs.Mechanic;
using ZorgmeldSysteem.Application.Interfaces.IServices;
using ZorgmeldSysteem.Domain.Entities;
using ZorgmeldSysteem.Domain.Enums;
using ZorgmeldSysteem.Persistence.Context;

namespace ZorgmeldSysteem.Persistence.Services
{
    public class MechanicService : IMechanicService
    {
        private readonly ZorgmeldContext _context;

        public MechanicService(ZorgmeldContext context)
        {
            _context = context;
        }

        public async Task<MechanicDto?> GetByIdAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.UserCompanies)
                    .ThenInclude(uc => uc.Company)
                .FirstOrDefaultAsync(u => u.UserID == id && u.UserLevel == UserLevel.Mechanic);

            if (user == null)
                return null;

            return MapToDto(user);
        }

        public async Task<IEnumerable<MechanicDto>> GetAllAsync()
        {
            var mechanics = await _context.Users
                .Include(u => u.UserCompanies)
                    .ThenInclude(uc => uc.Company)
                .Where(u => u.UserLevel == UserLevel.Mechanic)
                .ToListAsync();

            return mechanics.Select(MapToDto);
        }

        public async Task<IEnumerable<MechanicDto>> GetActiveAsync()
        {
            var mechanics = await _context.Users
                .Include(u => u.UserCompanies)
                    .ThenInclude(uc => uc.Company)
                .Where(u => u.UserLevel == UserLevel.Mechanic && u.IsActive)
                .ToListAsync();

            return mechanics.Select(MapToDto);
        }

        public async Task<IEnumerable<MechanicDto>> GetByTypeAsync(MechanicType type)
        {
            var mechanics = await _context.Users
                .Include(u => u.UserCompanies)
                    .ThenInclude(uc => uc.Company)
                .Where(u => u.UserLevel == UserLevel.Mechanic && u.MechanicType == type)
                .ToListAsync();

            return mechanics.Select(MapToDto);
        }

        public async Task<MechanicDto> CreateAsync(CreateMechanicDto createDto)
        {
            // Validate company exists
            var company = await _context.Companies.FindAsync(createDto.CompanyID);
            if (company == null)
                throw new InvalidOperationException($"Bedrijf met ID {createDto.CompanyID} niet gevonden");

            // Check if email already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == createDto.Email);
            if (existingUser != null)
                throw new InvalidOperationException($"Email {createDto.Email} is al in gebruik");

            // Split name
            var nameParts = createDto.Name.Split(' ', 2);
            var firstName = nameParts.Length > 0 ? nameParts[0] : createDto.Name;
            var lastName = nameParts.Length > 1 ? nameParts[1] : "";

            // Create user with mechanic role
            var user = new User
            {
                Email = createDto.Email,
                Username = createDto.Email.Split('@')[0],
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(createDto.TempPassword),
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = createDto.Phonenumber,
                UserLevel = UserLevel.Mechanic,
                MechanicType = createDto.Type,
                IsActive = true,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "System"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Link user to company
            var userCompany = new UserCompany
            {
                UserID = user.UserID,
                CompanyID = createDto.CompanyID,
                IsActive = true,
                AddedOn = DateTime.UtcNow
            };

            _context.UserCompanies.Add(userCompany);
            await _context.SaveChangesAsync();

            // Reload with includes
            return (await GetByIdAsync(user.UserID))!;
        }

        public async Task<MechanicDto> UpdateAsync(int id, UpdateMechanicDto updateDto)
        {
            var user = await _context.Users
                .Include(u => u.UserCompanies)
                .FirstOrDefaultAsync(u => u.UserID == id && u.UserLevel == UserLevel.Mechanic);

            if (user == null)
                throw new InvalidOperationException($"Monteur met ID {id} niet gevonden");

            // Update only provided fields
            if (!string.IsNullOrEmpty(updateDto.Name))
            {
                var nameParts = updateDto.Name.Split(' ', 2);
                user.FirstName = nameParts[0];
                user.LastName = nameParts.Length > 1 ? nameParts[1] : "";
            }

            if (!string.IsNullOrEmpty(updateDto.Email))
                user.Email = updateDto.Email;

            if (updateDto.Phonenumber != null)
                user.PhoneNumber = updateDto.Phonenumber;

            if (updateDto.Type.HasValue)
                user.MechanicType = updateDto.Type.Value;

            if (updateDto.IsActive.HasValue)
                user.IsActive = updateDto.IsActive.Value;

            user.ChangedOn = DateTime.UtcNow;
            user.ChangedBy = !string.IsNullOrEmpty(updateDto.ChangedBy) ? updateDto.ChangedBy : "System";

            // Update company if provided
            if (updateDto.CompanyID.HasValue)
            {
                var existingLink = user.UserCompanies.FirstOrDefault(uc => uc.IsActive);
                if (existingLink != null && existingLink.CompanyID != updateDto.CompanyID.Value)
                {
                    existingLink.IsActive = false;

                    var newLink = new UserCompany
                    {
                        UserID = user.UserID,
                        CompanyID = updateDto.CompanyID.Value,
                        IsActive = true,
                        AddedOn = DateTime.UtcNow
                    };
                    _context.UserCompanies.Add(newLink);
                }
            }

            await _context.SaveChangesAsync();

            return (await GetByIdAsync(id))!;
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null || user.UserLevel != UserLevel.Mechanic)
                throw new InvalidOperationException($"Monteur met ID {id} niet gevonden");

            // Soft delete
            user.IsActive = false;
            user.ChangedOn = DateTime.UtcNow;
            user.ChangedBy = "System";

            await _context.SaveChangesAsync();
        }

        private static MechanicDto MapToDto(User user)
        {
            var activeCompany = user.UserCompanies?.FirstOrDefault(uc => uc.IsActive);

            return new MechanicDto
            {
                MechanicID = user.UserID,
                Name = $"{user.FirstName} {user.LastName}".Trim(),
                Email = user.Email,
                Phonenumber = user.PhoneNumber ?? "",
                Type = user.MechanicType ?? MechanicType.InternalGeneral, 
                IsActive = user.IsActive,
                CompanyID = activeCompany?.CompanyID,
                CompanyName = activeCompany?.Company?.Name,
                CreatedOn = user.CreatedOn,
                CreatedBy = user.CreatedBy ?? "System"
            };
        }
    }
}