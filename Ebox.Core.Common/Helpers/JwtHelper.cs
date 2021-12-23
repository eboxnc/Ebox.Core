
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Ebox.Core.Common.Helpers
{
    public class JwtHelper
    {
        private static JwtConfig _jwtConfig = new JwtConfig();
        private static List<string> InvalidateTokens = new List<string>();
        public JwtHelper(IConfiguration configuration)
        {
            configuration.GetSection("JwtConfig").Bind(_jwtConfig);
        }

        /// <summary>
        /// 颁发JWT字符串
        /// </summary>
        /// <param name="tokenModel"></param>
        /// <returns></returns>
        public string IssueJwt(TokenModelJwt tokenModel)
        {      
            var claims = new List<Claim>
              {
                 /*
                 * 特别重要：
                   1、这里将用户的部分信息，比如 uid 存到了Claim 中，如果你想知道如何在其他地方将这个 uid从 Token 中取出来，请看下边的SerializeJwt() 方法，或者在整个解决方案，搜索这个方法，看哪里使用了！
                   2、你也可以研究下 HttpContext.User.Claims ，具体的你可以看看 Policys/PermissionHandler.cs 类中是如何使用的。
                 */                

                new Claim(JwtRegisteredClaimNames.Jti, tokenModel.Uid.ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, $"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}"),
                new Claim(JwtRegisteredClaimNames.Nbf,$"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}") ,
                //这个就是过期时间，目前是过期1000秒，可自定义，注意JWT有自己的缓冲过期时间
                new Claim (JwtRegisteredClaimNames.Exp,$"{new DateTimeOffset(DateTime.Now.AddSeconds(1000)).ToUnixTimeSeconds()}"),
                new Claim(JwtRegisteredClaimNames.Iss,_jwtConfig.Issuer),
                new Claim(JwtRegisteredClaimNames.Aud,_jwtConfig.Audience),
                
                //new Claim(ClaimTypes.Role,tokenModel.Role),//为了解决一个用户多个角色(比如：Admin,System)，用下边的方法
               };

            // 可以将一个用户的多个角色全部赋予；
            // 作者：DX 提供技术支持；
            claims.AddRange(tokenModel.Role.Split(',').Select(s => new Claim(ClaimTypes.Role, s)));


            //秘钥 (SymmetricSecurityKey 对安全性的要求，密钥的长度太短会报出异常)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: _jwtConfig.Issuer,
                claims: claims,
                signingCredentials: creds);

            var jwtHandler = new JwtSecurityTokenHandler();
            var encodedJwt = jwtHandler.WriteToken(jwt);

            return encodedJwt;
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="jwtStr"></param>
        /// <returns></returns>
        public TokenModelJwt SerializeJwt(string jwtStr)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(jwtStr);
            object role;
            try
            {
                jwtToken.Payload.TryGetValue(ClaimTypes.Role, out role);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            var tm = new TokenModelJwt
            {
                Uid = int.Parse(jwtToken.Id),
                Role = role != null ? role.ToString() : "",
            };
            return tm;
        }

        public bool ValidateToken(string Token, out Dictionary<string, string> Clims)
        {
            Clims = new Dictionary<string, string>();
            if (InvalidateTokens.Contains(Token))
            {
                return false;
            }
            ClaimsPrincipal principal = null;
            if (string.IsNullOrWhiteSpace(Token))
            {
                return false;
            }
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            
            try
            {
                JwtSecurityToken jwt = handler.ReadJwtToken(Token);
                if (jwt == null)
                {
                    return false;
                }
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                TokenValidationParameters validationParameters = new TokenValidationParameters
                {
                    RequireExpirationTime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtConfig.Secret)),
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuer = true,//是否验证Issuer
                    ValidateAudience = true,//是否验证Audience
                    ValidateLifetime = true,//是否验证失效时间
                    ValidateIssuerSigningKey = true,//是否验证SecurityKey
                    ValidAudience = _jwtConfig.Audience,
                    ValidIssuer = _jwtConfig.Issuer
                };
                principal = handler.ValidateToken(Token, validationParameters, out SecurityToken securityToken);
                foreach (Claim item in principal.Claims)
                {
                    Clims.Add(item.Type, item.Value);
                }
                return true;
            }
            catch (SecurityTokenInvalidLifetimeException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public string RenewToken(string token)
        {
            token = token.Replace("Bearer ", "");
            if (ValidateToken(token, out var Clims))
            {
                if (Clims.Keys.FirstOrDefault(o => o == "exp") != null)
                {
                    var start = new DateTime(1970, 1, 1, 0, 0, 0);
                    var timespanStart = long.Parse(Clims["exp"]);//token过期时间
                    var expDate = start.AddSeconds(timespanStart).ToLocalTime();
                    var o = expDate - DateTime.Now;
                    if (o.TotalSeconds > 0 && o.TotalSeconds < _jwtConfig.RenewSeconds)
                    {
                        var model = this.SerializeJwt(token);
                        InvalidToken(token);
                        return this.IssueJwt(model);
                    }
                }
            }
            return string.Empty;
        }

        public void InvalidToken(string token)
        {
            InvalidateTokens.Add(GetNoBearerToken(token));

        }

        public bool IsInvalidToken(string token)
        {

            return InvalidateTokens.Contains(GetNoBearerToken(token));
        }

        public string GetNoBearerToken(string token)
        {
            return token.Replace("Bearer ", "");
        }
    }

    /// <summary>
    /// 令牌
    /// </summary>
    public class TokenModelJwt
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Uid { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        public string Role { get; set; }
        /// <summary>
        /// 职能
        /// </summary>
        public string Work { get; set; }

    }

    public class JwtConfig
    {
        public string Secret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }

        /// <summary>
        /// 续期时间
        /// </summary>
        public int RenewSeconds { get; set; }
    }
}
