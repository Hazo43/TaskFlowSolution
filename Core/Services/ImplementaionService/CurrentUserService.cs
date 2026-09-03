using Microsoft.AspNetCore.Http;
using Services.Abstraction.Interfaces;
using System.Security.Claims;

namespace Services.ImplementaionService
{

      // Token request من ال UserId دا يجبلي ال class مهمه ال
     // مين المستخدم الحالي services بيعرف ال class ال
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public int UserId
        {
            get
            {
                // Token من ال UserId(OwnerId) عشان نجيب ال http request من user بتجيب ال
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId is null)
                    throw new Exception("User Is is not found in token");
              
                // Parse ف عملنا int واحنا عاوزينها string بتكون clim قيمه ال
                return int.Parse(userId);

            }
        }
    }
}



