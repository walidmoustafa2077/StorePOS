using StorePOS.Domain.DTOs;
using StorePOS.Domain.Models;
using System.Linq.Expressions;

namespace StorePOS.Domain.Extensions
{
    public static class UserMappingExtensions
    {
        // EF-translatable projection
        public static readonly Expression<Func<User, UserReadDto>> ToReadDtoExpr =
            u => new UserReadDto(
                u.Id, 
                u.Username, 
                u.Email, 
                u.FirstName, 
                u.LastName, 
                u.PhoneNumber, 
                u.Role.ToString(), 
                u.IsActive, 
                u.CreatedAt, 
                u.LastLoginAt);

        // Use this inside IQueryable pipelines so translation happens server-side
        public static IQueryable<UserReadDto> SelectReadDto(this IQueryable<User> query) =>
            query.Select(ToReadDtoExpr);

        // Use these only after materialization (in-memory)
        public static UserReadDto ToReadDto(this User u) =>
            ToReadDtoExpr.Compile().Invoke(u);
    }
}
