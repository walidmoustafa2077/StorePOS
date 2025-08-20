using Microsoft.EntityFrameworkCore;
using StorePOS.Domain.Data.Repositories.Interfaces;
using StorePOS.Domain.DTOs;
using StorePOS.Domain.Extensions;
using StorePOS.Domain.Helpers;
using StorePOS.Domain.Models;

namespace StorePOS.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _uow;

        public UserService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Task<List<UserReadDto>> SearchAsync(string? q, CancellationToken ct = default)
        {
            var query = _uow.Users.Query();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(u =>
                    EF.Functions.Like(u.Username, $"%{q}%") ||
                    EF.Functions.Like(u.Email, $"%{q}%") ||
                    EF.Functions.Like(u.FirstName, $"%{q}%") ||
                    EF.Functions.Like(u.LastName, $"%{q}%"));
            }

            return query
                .OrderBy(u => u.Username)
                .Take(100)
                .SelectReadDto()
                .ToListAsync(ct);
        }

        public Task<UserReadDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return _uow.Users.Query()
                .Where(u => u.Id == id)
                .SelectReadDto()
                .FirstOrDefaultAsync(ct);
        }

        public Task<UserReadDto?> GetByUsernameAsync(string username, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(username))
                return Task.FromResult<UserReadDto?>(null);

            return _uow.Users.Query()
                .Where(u => u.Username == username.Trim())
                .SelectReadDto()
                .FirstOrDefaultAsync(ct);
        }

        public Task<UserReadDto?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(email))
                return Task.FromResult<UserReadDto?>(null);

            return _uow.Users.Query()
                .Where(u => u.Email == email.Trim().ToLower())
                .SelectReadDto()
                .FirstOrDefaultAsync(ct);
        }

        public async Task<ServiceResult<UserReadDto>> CreateAsync(UserCreateDto dto, CancellationToken ct = default)
        {
            var username = dto.Username?.Trim() ?? string.Empty;
            var email = dto.Email?.Trim().ToLower() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(username))
                return ServiceResult<UserReadDto>.BadRequest("Username is required.");

            if (string.IsNullOrWhiteSpace(email))
                return ServiceResult<UserReadDto>.BadRequest("Email is required.");

            if (string.IsNullOrWhiteSpace(dto.Password))
                return ServiceResult<UserReadDto>.BadRequest("Password is required.");

            if (dto.Password.Length < 6)
                return ServiceResult<UserReadDto>.BadRequest("Password must be at least 6 characters long.");

            // Check if username or email already exists
            var exists = await _uow.Users.ExistsAsync(username, email, ct);
            if (exists)
                return ServiceResult<UserReadDto>.Conflict("Username or email already exists.");

            var user = new User
            {
                Username = username,
                Email = email,
                FirstName = dto.FirstName?.Trim() ?? string.Empty,
                LastName = dto.LastName?.Trim() ?? string.Empty,
                PasswordHash = PasswordHelper.HashPassword(dto.Password),
                PhoneNumber = dto.PhoneNumber?.Trim(),
                Role = Enum.Parse<Enums.UserRole>(dto.Role ?? "Cashier"),
                IsActive = dto.IsActive,
                CreatedAt = DateTimeOffset.Now
            };

            await _uow.Users.AddAsync(user, ct);
            await _uow.SaveChangesAsync(ct);

            var userDto = user.ToReadDto();
            return ServiceResult<UserReadDto>.Created(userDto);
        }

        public async Task<ServiceResult<UserReadDto>> UpdateAsync(int id, UserUpdateDto dto, CancellationToken ct = default)
        {
            var user = await _uow.Users.GetByIdAsync(id, false, ct);
            if (user is null)
                return ServiceResult<UserReadDto>.NotFound("User not found.");

            var username = dto.Username?.Trim() ?? string.Empty;
            var email = dto.Email?.Trim().ToLower() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(username))
                return ServiceResult<UserReadDto>.BadRequest("Username is required.");

            if (string.IsNullOrWhiteSpace(email))
                return ServiceResult<UserReadDto>.BadRequest("Email is required.");

            // Check if username or email already exists (excluding current user)
            var exists = await _uow.Users.ExistsAsync(username, email, id, ct);
            if (exists)
                return ServiceResult<UserReadDto>.Conflict("Username or email already exists.");

            user.Username = username;
            user.Email = email;
            user.FirstName = dto.FirstName?.Trim() ?? string.Empty;
            user.LastName = dto.LastName?.Trim() ?? string.Empty;
            user.PhoneNumber = dto.PhoneNumber?.Trim();
            user.Role = Enum.Parse<Enums.UserRole>(dto.Role ?? "Cashier");
            user.IsActive = dto.IsActive;
            user.UpdatedAt = DateTimeOffset.Now;

            _uow.Users.Update(user);
            await _uow.SaveChangesAsync(ct);

            var userDto = user.ToReadDto();
            return ServiceResult<UserReadDto>.Ok(userDto);
        }

        public async Task<ServiceResult<bool>> ChangePasswordAsync(int id, UserChangePasswordDto dto, CancellationToken ct = default)
        {
            var user = await _uow.Users.GetByIdAsync(id, false, ct);
            if (user is null)
                return ServiceResult<bool>.NotFound("User not found.");

            if (string.IsNullOrWhiteSpace(dto.NewPassword))
                return ServiceResult<bool>.BadRequest("New password is required.");

            if (dto.NewPassword.Length < 6)
                return ServiceResult<bool>.BadRequest("New password must be at least 6 characters long.");

            // Verify current password
            if (!PasswordHelper.VerifyPassword(dto.CurrentPassword, user.PasswordHash))
                return ServiceResult<bool>.BadRequest("Current password is incorrect.");

            user.PasswordHash = PasswordHelper.HashPassword(dto.NewPassword);
            user.UpdatedAt = DateTimeOffset.Now;

            _uow.Users.Update(user);
            await _uow.SaveChangesAsync(ct);

            return ServiceResult<bool>.Ok(true);
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id, CancellationToken ct = default)
        {
            var user = await _uow.Users.GetByIdAsync(id, false, ct);
            if (user is null)
                return ServiceResult<bool>.NotFound("User not found.");

            _uow.Users.Remove(user);
            await _uow.SaveChangesAsync(ct);

            return ServiceResult<bool>.Ok(true);
        }

        public async Task<ServiceResult<UserReadDto>> LoginAsync(UserLoginDto dto, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(dto.UsernameOrEmail))
                return ServiceResult<UserReadDto>.BadRequest("Username or email is required.");

            if (string.IsNullOrWhiteSpace(dto.Password))
                return ServiceResult<UserReadDto>.BadRequest("Password is required.");

            var user = await _uow.Users.GetByUsernameOrEmailAsync(dto.UsernameOrEmail, ct);
            if (user is null)
                return ServiceResult<UserReadDto>.BadRequest("Invalid username/email or password.");

            if (!user.IsActive)
                return ServiceResult<UserReadDto>.BadRequest("User account is inactive.");

            if (!PasswordHelper.VerifyPassword(dto.Password, user.PasswordHash))
                return ServiceResult<UserReadDto>.BadRequest("Invalid username/email or password.");

            // Update last login time - get entity for tracking
            var trackedUser = await _uow.Users.GetByIdAsync(user.Id, false, ct);
            if (trackedUser != null)
            {
                trackedUser.LastLoginAt = DateTimeOffset.Now;
                _uow.Users.Update(trackedUser);
                await _uow.SaveChangesAsync(ct);
            }

            var userDto = user.ToReadDto();
            return ServiceResult<UserReadDto>.Ok(userDto);
        }

        public async Task<ServiceResult<bool>> UpdateLastLoginAsync(int id, CancellationToken ct = default)
        {
            var user = await _uow.Users.GetByIdAsync(id, false, ct);
            if (user is null)
                return ServiceResult<bool>.NotFound("User not found.");

            user.LastLoginAt = DateTimeOffset.Now;
            _uow.Users.Update(user);
            await _uow.SaveChangesAsync(ct);

            return ServiceResult<bool>.Ok(true);
        }
    }
}
