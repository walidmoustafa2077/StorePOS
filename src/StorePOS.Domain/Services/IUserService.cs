using StorePOS.Domain.DTOs;

namespace StorePOS.Domain.Services
{
    public interface IUserService
    {
        Task<List<UserReadDto>> SearchAsync(string? q, CancellationToken ct = default);
        Task<UserReadDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<UserReadDto?> GetByUsernameAsync(string username, CancellationToken ct = default);
        Task<UserReadDto?> GetByEmailAsync(string email, CancellationToken ct = default);

        Task<ServiceResult<UserReadDto>> CreateAsync(UserCreateDto dto, CancellationToken ct = default);
        Task<ServiceResult<UserReadDto>> UpdateAsync(int id, UserUpdateDto dto, CancellationToken ct = default);
        Task<ServiceResult<bool>> ChangePasswordAsync(int id, UserChangePasswordDto dto, CancellationToken ct = default);
        Task<ServiceResult<bool>> DeleteAsync(int id, CancellationToken ct = default);
        
        // Authentication methods
        Task<ServiceResult<UserReadDto>> LoginAsync(UserLoginDto dto, CancellationToken ct = default);
        Task<ServiceResult<bool>> UpdateLastLoginAsync(int id, CancellationToken ct = default);
    }
}
