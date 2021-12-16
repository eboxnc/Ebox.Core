
using Ebox.Core.Common.Helpers;
using Ebox.Core.Model;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ebox.Core.Identity
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            try
            {
                Func<string, bool> validator = context.Password.Length == 32 ?
                    new Func<string, bool>(t => t == context.Password) :
                    new Func<string, bool>(t => t == EncryptHelper.Create(context.Password));
                // var user = await _adminService.CheckLogin(context.UserName, validator);
                var user = new User() { UserId = 1, UserName = "张三" };
                if (user != null)
                {
                    context.Result = new GrantValidationResult(
                         subject: user.UserId.ToString(),
                         authenticationMethod: "custom",
                         claims: await GetClaims(user));
                }
                else
                {
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "错误的用户名及密码");
                }
            }
            //catch (ClientNotificationException exp)
            //{
            //    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, exp.Message);
            //}
            catch (Exception exp)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "登录失败，未知的错误");
            }
        }

        private async Task<Claim[]> GetClaims(User user)
        {
            //var orgCode = user.SysOrg.Code;
            //var corpType = CorpHelper.GetCorpType(orgCode);
            //var subOrgCode = CorpHelper.GetSubOrgCode(corpType, orgCode);
            //var orgAttr = user.SysOrg.Attribute.ToString("D");
            //var subOrgId = (await service.GetOrg(subOrgCode)).OrgID;
            //var postIds = await service.GetPostByUserId(user.UserID);

            var claim = new Claim[]
            {
                new Claim("UserId",user.UserId.ToString()),
                new Claim(nameof(user.UserName),user.UserName.ToString()),
                //new Claim(SkiadClaimTypes.OrgCode, orgCode),
                //new Claim(SkiadClaimTypes.SubOrgId, subOrgId.ToString()),
                //new Claim(SkiadClaimTypes.SubOrgCode, subOrgCode),
                //new Claim(SkiadClaimTypes.OrgAttribute, orgAttr),
                //new Claim(SkiadClaimTypes.CorpType, corpType.ToString()),
                //new Claim(SkiadClaimTypes.PostIds, string.Join(",", postIds.Where(s => s != 0))),
            };
            return await Task.FromResult(claim);
        }
    }




}
