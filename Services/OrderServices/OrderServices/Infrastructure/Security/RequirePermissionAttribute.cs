using Microsoft.AspNetCore.Authorization;

namespace OrderServices.Infrastructure.Security
{
    public class RequirePermissionAttribute : AuthorizeAttribute
    {
        public RequirePermissionAttribute(string permission)
        {
            Policy = $"PERMISSION:{permission}";
        }
    }
}
