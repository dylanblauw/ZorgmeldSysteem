using Microsoft.EntityFrameworkCore;
using ZorgmeldSysteem.Application.DTOs.Auth;
using ZorgmeldSysteem.Domain.Entities;
using ZorgmeldSysteem.Domain.Enums;
using ZorgmeldSysteem.Persistence.Context;
using ZorgmeldSysteem.Persistence.Services;

namespace ZorgmeldSysteem.Persistence.Services
{
     public class AuthService
    {
        private readonly ZorgmeldContext _context;
        private readonly JwtService _jwtService;

        public AuthService(ZorgmeldContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto loginDto)
        {
            var user = await _context.Users
                .Include(u => u.UserCompanies)
                    .ThenInclude(uc => uc.Company)
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email && u.IsActive);

            if (user == null) return null;

            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                return null;

            user.LastLogin = DateTime.Now;
            await _context.SaveChangesAsync();

            var companyIds = user.UserCompanies
                .Where(uc => uc.IsActive)
                .Select(uc => uc.CompanyID)
                .ToList();

            var companyNames = user.UserCompanies
                .Where(uc => uc.IsActive)
                .Select(uc => uc.Company.Name)
                .ToList();

            var token = _jwtService.GenerateToken(user, companyIds);
            var expiration = DateTime.UtcNow.AddMinutes(480);

            return new AuthResponseDto
            {
                UserID = user.UserID,
                Email = user.Email,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                UserLevel = ((int)user.UserLevel),
                UserLevelName = user.UserLevelName,
                CompanyIDs = companyIds,
                CompanyNames = companyNames,
                Token = token,
                TokenExpiration = expiration
            };
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterRequestDto registerDto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
                return null;

            var username = registerDto.Email.Split('@')[0];
            if (await _context.Users.AnyAsync(u => u.Username == username))
                username = $"{username}{new Random().Next(1000, 9999)}";

            var company = await _context.Companies.FindAsync(registerDto.CompanyID);
            if (company == null) return null;

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            var user = new User
            {
                Email = registerDto.Email,
                Username = username,
                PasswordHash = passwordHash,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                PhoneNumber = registerDto.PhoneNumber,
                UserLevel = (UserLevel)registerDto.UserLevel,
                IsActive = true,
                CreatedOn = DateTime.Now,
                CreatedBy = "System"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var userCompany = new UserCompany
            {
                UserID = user.UserID,
                CompanyID = registerDto.CompanyID,
                AddedOn = DateTime.Now,
                IsActive = true
            };

            _context.UserCompanies.Add(userCompany);
            await _context.SaveChangesAsync();

            var token = _jwtService.GenerateToken(user, new List<int> { registerDto.CompanyID });
            var expiration = DateTime.UtcNow.AddMinutes(480);

            return new AuthResponseDto
            {
                UserID = user.UserID,
                Email = user.Email,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                UserLevel = ((int)user.UserLevel),
                UserLevelName = user.UserLevelName,
                CompanyIDs = new List<int> { registerDto.CompanyID },
                CompanyNames = new List<string> { company.Name },
                Token = token,
                TokenExpiration = expiration
            };
        }
    }
}