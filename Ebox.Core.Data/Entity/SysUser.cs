using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ebox.Core.Data.Entity
{
    [SugarTable("SysUser")]
    public class SysUser
    {

        /// <summary>
        /// 获取或设置人员ID。
        /// </summary>

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int UserID { get; set; }

        /// <summary>
        /// 获取或设置机构ID。
        /// </summary>

        public int OrgID { get; set; }

        /// <summary>
        /// 获取或设置姓名。
        /// </summary>

        public string Name { get; set; }

        /// <summary>
        /// 获取或设置账号。
        /// </summary>


        public string Account { get; set; }

        /// <summary>
        /// 获取或设置岗位名称。
        /// </summary>


        public string PostNames { get; set; }

        /// <summary>
        /// 获取或设置密码。
        /// </summary>


        public string Password { get; set; }

        /// <summary>
        /// 获取或设置手机号。
        /// </summary>


        public string Mobile { get; set; }

        /// <summary>
        /// 获取或设置邮箱。
        /// </summary>

        public string Email { get; set; }



        /// <summary>
        /// 获取或设置性别。
        /// </summary>


        public int Sex { get; set; }

        /// <summary>
        /// 获取或设置学历。
        /// </summary>


        public int? DegreeNo { get; set; }

        /// <summary>
        /// 获取或设置职称。
        /// </summary>


        public int? TitleNo { get; set; }

        /// <summary>
        /// 获取或设置拼音码。
        /// </summary>


        public string PyCode { get; set; }

        /// <summary>
        /// 获取或设置状态。
        /// </summary>

        public StateFlags State { get; set; }

        /// <summary>
        /// 获取或设置最近登录时间。
        /// </summary>


        public DateTime? LastLoginTime { get; set; }

        /// <summary>
        /// 获取或设置是否驾驶员。
        /// </summary>


        public bool IsDriver { get; set; }


        /// <summary>
        /// 获取或设置是否驾驶员。
        /// </summary>

        public onLineState IsOnline { get; set; }

        /// <summary>
        /// 获取或设置驾驶证号。
        /// </summary>


        public string DriverNo { get; set; }

        /// <summary>
        /// 获取或设置令牌。
        /// </summary>


        public string Token { get; set; }

        /// <summary>
        /// 获取或设置设备号。
        /// </summary>


        public string DeviceNo { get; set; }

    }
}
