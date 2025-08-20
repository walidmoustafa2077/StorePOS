using Microsoft.AspNetCore.Authorization;
using StorePOS.Domain.Enums;

namespace StorePOS.Api.Authorization
{
    public class RequireRoleAttribute : AuthorizeAttribute
    {
        public RequireRoleAttribute(params UserRole[] roles)
        {
            Roles = string.Join(",", roles);
        }
    }
}
