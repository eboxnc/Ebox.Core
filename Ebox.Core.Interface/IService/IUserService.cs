using Ebox.Core.Data;
using Ebox.Core.Data.Entity;
using Ebox.Core.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ebox.Core.Interface.IService
{
    public interface IUserService : IBaseRepository<SysUser>
    {
        Task<SysUser> CheckLogin(string account, Func<string, bool> validator);

        Task<UserInfo> GetUserInfo(int userId);
    }
}
