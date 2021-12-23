
using Ebox.Core.Common.Helpers;
using Ebox.Core.Data;
using Ebox.Core.Data.Entity;
using Ebox.Core.Extensions.Exception;
using Ebox.Core.Interface;
using Ebox.Core.Interface.IService;
using Ebox.Core.Model;
using Ebox.Core.Model.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Threading.Tasks;

namespace Ebox.Core.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : Controller
    {
        private ICacheManager _cacheManager;
        private JwtHelper _JwtHelper;
        private ILogger<UserController> _logger;

        private IUserService _userService;
        public UserController(ICacheManager cacheManager, JwtHelper jwtHelper, IUserService userService, ILogger<UserController> logger)
        {
            _cacheManager = cacheManager;
            _JwtHelper = jwtHelper;
            _userService = userService;
            _logger = logger;
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

        [HttpGet]
        public async Task<IList> GetUsers()
        {
            return await _userService.Query(s => s.UserID < 100);
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
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Login(LoginInfo info)
        {
            try
            {
                var user = await _userService.CheckLogin(info.UserName, t => EncryptHelper.Validate(info.Password, t));
                if (user != null)
                {
                    //if (_appSettingOption.FirstLoginCheckPwd && string.Compare(password, "123456") == 0)
                    //{
                    //    //如果是简单密码，强制改密码后再进行session写入
                    //    return Json(Result.Success("系统检测到您账号是第一次登陆，需要验证手机号码。", user.Mobile));
                    //}
                    TokenModelJwt tokenModel = new TokenModelJwt { Uid = user.UserID, Role = "Admin", Work = "admin" };
                    var jwtStr = _JwtHelper.IssueJwt(tokenModel);

                    return Json(Result.Success("登录成功", new { token = jwtStr }));
                }
                return Json(Result.Fail("登录失败，用户名或密码不匹配，或帐号被停用。"));
            }
            catch (ClientNotificationException exp)
            {
                return Json(Result.Fail(exp.Message));
            }
            catch (Exception exp)
            {
                _logger.LogError("登录失败", exp);
                return Json(Result.Fail("登录失败，发生未知的错误，请查看系统日志。"));
            }

        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> LoginOut()
        {
            var token = Request.Headers["Authorization"];

            _JwtHelper.InvalidToken(_JwtHelper.GetNoBearerToken(token));

            return Json(Result.Success("成功"));
        }

        [Authorize]
        [HttpGet]
        public async Task<JsonResult> Info()
        {
            var token = Request.Headers["Authorization"];

            var user = _JwtHelper.SerializeJwt(_JwtHelper.GetNoBearerToken(token));
            var userinfo = await _userService.GetUserInfo(user.Uid);
            return Json(Result.Success("成功", userinfo));
        }
    }
}
