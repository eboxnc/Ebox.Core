using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Ebox.Core.Common.Helpers
{
    public class EncryptHelper
    {
        //加盐格式
        private const string MD5_SALT = "&^*y3{0}MnIarJ";

        /// <summary>
        /// 创建密文。
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string Create(string password)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            return BitConverter.ToString(md5.ComputeHash(Encoding.Default.GetBytes(string.Format(MD5_SALT, password)))).Replace("-", "").ToUpper();
        }

        /// <summary>
        /// 验证密文是否正确。
        /// </summary>
        /// <param name="password"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        public static bool Validate(string password, string actual)
        {
            return Create(password) == actual;
        }
    }
}
