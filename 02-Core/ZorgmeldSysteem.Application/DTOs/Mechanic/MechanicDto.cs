using ZorgmeldSysteem.Domain.Enums;

namespace ZorgmeldSysteem.Application.DTOs.Mechanic
{
    /// <summary>
    /// MechanicDto is now an ALIAS for User with UserLevel = 3
    /// This maintains backward compatibility with existing frontend code
    /// </summary>
    public class MechanicDto
    {
        public int MechanicID { get; set; }  // Maps to UserID
        public string Name { get; set; } = string.Empty;  // Maps to FullName
        public string Email { get; set; } = string.Empty;
        public string Phonenumber { get; set; } = string.Empty;
        public MechanicType Type { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public int? CompanyID { get; set; }
        public string? CompanyName { get; set; }

        // Conversion helper
        public static MechanicDto FromUser(Domain.Entities.User user, int? primaryCompanyId = null, string? companyName = null)
        {
            if (user.UserLevel != UserLevel.Mechanic)
                throw new InvalidOperationException("Can only convert Users with UserLevel = 3 (Mechanic) to MechanicDto");

            return new MechanicDto
            {
                MechanicID = user.UserID,
                Name = user.FullName,
                Email = user.Email,
                Phonenumber = user.PhoneNumber ?? string.Empty,
                Type = user.MechanicType ?? MechanicType.InternalGeneral,
                IsActive = user.IsActive,
                CreatedOn = user.CreatedOn,
                CreatedBy = user.CreatedBy ?? string.Empty,
                CompanyID = primaryCompanyId,
                CompanyName = companyName
            };
        }

        // Reverse conversion for creating/updating
        public static Domain.Entities.User ToUser(MechanicDto dto, string passwordHash)
        {
            var nameParts = dto.Name.Split(' ', 2);
            var firstName = nameParts.Length > 0 ? nameParts[0] : dto.Name;
            var lastName = nameParts.Length > 1 ? nameParts[1] : "";

            return new Domain.Entities.User
            {
                UserID = dto.MechanicID > 0 ? dto.MechanicID : 0,
                Email = dto.Email,
                Username = dto.Email.Split('@')[0],
                PasswordHash = passwordHash,
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = dto.Phonenumber,
                UserLevel = UserLevel.Mechanic,
                MechanicType = dto.Type,
                IsActive = dto.IsActive,
                CreatedBy = dto.CreatedBy
            };
        }
    }
}