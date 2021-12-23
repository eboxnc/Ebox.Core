using Ebox.Core.Data;
using Ebox.Core.Data.Entity;
using Ebox.Core.Extensions.Exception;
using Ebox.Core.Interface.IService;
using Ebox.Core.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ebox.Core.Interface.Service
{
    public class UserService : BaseRepository<SysUser>, IUserService
    {
        public UserService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public async Task<SysUser> CheckLogin(string account, Func<string, bool> validator)
        {
            var user = (await base.Query(s => (s.Account == account || s.Mobile == account) && s.State == StateFlags.Enabled)).LastOrDefault();
            if (user == null || !validator(user.Password))
            {
                throw new ClientNotificationException("你的帐号不存在或密码有误");
            }

            if (user.State == 0)
            {
                throw new ClientNotificationException("你的帐号已被停用。");
            }

            //if (user.SysOrg == null)
            //{
            //    throw new ClientNotificationException("你所属的机构失效，请联系管理员。");
            //}
            //user.IsOnline = onLineState.On;
            //user.LastLoginTime = DateTime.Now;
            await Update(s => new SysUser() { IsOnline = onLineState.On, LastLoginTime = DateTime.Now }, s => s.UserID == user.UserID);

            return user;
        }

        public async Task<UserInfo> GetUserInfo(int userId)
        {
            var user = await QueryById(userId);
            if (user == null)
            {
                throw new ClientNotificationException("用户不存在");
            }

            return new UserInfo()
            {
                Avatar = "https://wpimg.wallstcn.com/f778738c-e4f8-4870-b634-56703b4acafe.gif",
                Introduction = "我是管理员",
                Name = user.Name,
                Roles = new List<string> { "admin" }
            };
        }
    }
}
