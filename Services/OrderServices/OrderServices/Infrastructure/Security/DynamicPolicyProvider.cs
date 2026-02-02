using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace OrderServices.Infrastructure.Security
{
    public class DynamicPolicyProvider : IAuthorizationPolicyProvider
    {
        private const string PREFIX = "PERMISSION:";
        private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;

        public DynamicPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
            => _fallbackPolicyProvider.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
            => _fallbackPolicyProvider.GetFallbackPolicyAsync();

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(PREFIX))
            {
                var permission = policyName.Substring(PREFIX.Length);

                var policy = new AuthorizationPolicyBuilder()
                    .AddRequirements(new PermissionRequirement(permission))
                    .Build();

                return Task.FromResult<AuthorizationPolicy?>(policy);
            }

            return _fallbackPolicyProvider.GetPolicyAsync(policyName);
        }
    }

}

