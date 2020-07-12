using Kontokorrent.ApiModels.v2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kontokorrent.Controllers.v2
{
    [Route("api/v2/userinfo")]
    [Authorize]
    public class UserInfoController : Controller
    {
        [HttpGet]
        public IActionResult UserInfo()
        {
            return Ok(new UserInfoResponse()
            {
                Id = User.GetId().Id
            });
        }
    }
}