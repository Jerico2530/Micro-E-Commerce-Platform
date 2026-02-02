using Microsoft.AspNetCore.Authorization;

namespace ProductServices.Infrastructure.Security
{
    public class RequirePermissionAttribute : AuthorizeAttribute
    {
        public RequirePermissionAttribute(string permission)
        {
            Policy = $"PERMISSION:{permission}";
        }
    }
}
