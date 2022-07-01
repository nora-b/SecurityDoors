using System.Security.Claims;
using API.Constants;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Infrastructure.Security
{
    public class IsAdminRequirement : IAuthorizationRequirement
    {

    }
    public class IsAdminRequirementHandler : AuthorizationHandler<IsAdminRequirement>
    {
        private readonly DataContext _dataContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;
        public IsAdminRequirementHandler(DataContext dataContext, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _dataContext = dataContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsAdminRequirement requirement)
        {
            var username = context.User.FindFirst(ClaimTypes.Email);

            if (username == null) return Task.CompletedTask;

            var userId = username.Value;

            if (userId == null) return Task.CompletedTask;

            var user =  _userManager.FindByNameAsync(userId);

            var userRole =  _userManager.GetRolesAsync(user.Result);

            if (!userRole.Result.Contains(AppConstants.AdminRoleName)) return Task.CompletedTask;

            context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}