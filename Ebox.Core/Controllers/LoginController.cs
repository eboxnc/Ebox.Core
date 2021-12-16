
using Ebox.Core.Common.Helpers;
using Ebox.Core.Interface.IService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;

namespace Ebox.Core.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class LoginController : Controller
    {
        private ICacheManager _cacheManager;
        private JwtHelper _JwtHelper;
        public LoginController(ICacheManager cacheManager, JwtHelper jwtHelper)
        {
            _cacheManager = cacheManager;
            _JwtHelper = jwtHelper;
        }

        [HttpGet]
        public async Task<object> GetJwtStr(string name, string pass)
        {
            // 将用户id和角色名，作为单独的自定义变量封装进 token 字符串中。
            TokenModelJwt tokenModel = new TokenModelJwt { Uid = 1, Role = "Admin" };
            var jwtStr = _JwtHelper.IssueJwt(tokenModel);//登录，获取到一定规则的 Token 令牌
            var suc = true;
            return Ok(new
            {
                success = suc,
                token = jwtStr
            });
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost(nameof(Register))]
        public async Task<IActionResult> Register(Ebox.Core.Model.User model)
        {
            //if (!await _userService.Register(model))
            //{
            //    return Ok("用户已存在");
            //}
            return Ok("创建成功");
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("Login", Name = nameof(Login))]
        public async Task<IActionResult> Login(Model.User user)
        {
            //判断账号密码是否正确
            // var userId = await _userService.Login(model);
            if (user.UserName != "admin")
                return Ok("账号或密码错误！");

            //登录成功进行jwt加密
            //  var user = await _userService.GetOneByIdAsync(userId);
            TokenModelJwt tokenModel = new TokenModelJwt { Uid = 1, Role = "Admin", Work = "admin" };
            var jwtStr = _JwtHelper.IssueJwt(tokenModel);
            //   _cacheManager.Add()
            return Ok(jwtStr);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> LoginOut()
        {
            var token = Request.Headers["Authorization"];

            _JwtHelper.InvalidToken(_JwtHelper.GetNoBearerToken(token));

            return Ok("注销成功");
        }

    }
}
