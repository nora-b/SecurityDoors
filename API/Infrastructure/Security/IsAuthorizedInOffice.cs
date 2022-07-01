using System.Security.Claims;
using API.Constants;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Persistence;

namespace API.Infrastructure.Security
{
    public class IsAuthorizedInOffice : IAuthorizationRequirement
    {

    }

    public class IsAuthorizedInOfficeHandler : AuthorizationHandler<IsAuthorizedInOffice>
    {
        private readonly DataContext _dataContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;
        public IsAuthorizedInOfficeHandler(DataContext dataContext, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _dataContext = dataContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsAuthorizedInOffice requirement)
        {
            var user = context.User;
            var claimTunnelExpirationDate = user.FindFirst(x => x.Type == AppConstants.TagTunnelExpirationName);
            var claimOfficeExpirationDate = user.FindFirst(x => x.Type == AppConstants.TagOfficeExpirationName);

            bool hasTunnelExpired = false;
            if(claimTunnelExpirationDate is not null){
                _= DateTimeOffset.TryParse(claimTunnelExpirationDate.Value, out var tunnelExpirationDate);
                if (tunnelExpirationDate != default && tunnelExpirationDate <=DateTimeOffset.UtcNow){
                    hasTunnelExpired = true;
                }
            }
            
            bool hasOfficeExpired = false;
            if(claimOfficeExpirationDate is not null){
                _= DateTimeOffset.TryParse(claimOfficeExpirationDate.Value, out var officeExpirationDate);
                if (officeExpirationDate != default && officeExpirationDate <=DateTimeOffset.UtcNow){
                    hasOfficeExpired = true;
                }
            }
        
            if (!user.HasClaim(AppConstants.TagTunnelStatusName, TagStatus.Active.ToString()) || 
                !user.HasClaim(AppConstants.TagTunnelIsAuthorizedName, "True") || hasTunnelExpired) return Task.CompletedTask;

            if (!user.HasClaim(AppConstants.TagOfficeStatusName, TagStatus.Active.ToString()) || 
                !user.HasClaim(AppConstants.TagOfficeIsAuthorizedName, "True") || hasOfficeExpired) return Task.CompletedTask;

            context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}