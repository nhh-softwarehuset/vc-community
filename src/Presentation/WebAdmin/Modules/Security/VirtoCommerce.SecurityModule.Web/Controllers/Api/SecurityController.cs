﻿using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using VirtoCommerce.Foundation.Security.Model;
using VirtoCommerce.SecurityModule.Web.Configs;
using VirtoCommerce.SecurityModule.Web.Data;
using VirtoCommerce.SecurityModule.Web.Models;

namespace VirtoCommerce.SecurityModule.Web.Controllers
{
    [RoutePrefix("api/security")]
	public class SecurityController : ApiController
	{
        private readonly Func<IFoundationSecurityRepository> _securityRepository;
        private readonly Func<IFoundationCustomerRepository> _customerRepository;

        private IAuthenticationManager _authenticationManager;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public SecurityController(Func<IFoundationSecurityRepository> securityRepository, Func<IFoundationCustomerRepository> customerRepository)
        {
            _securityRepository = securityRepository;
            _customerRepository = customerRepository;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? (_signInManager = HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>());
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? (_userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>());
            }
        }

        public IAuthenticationManager AuthenticationManager
        {
            get
            {
                return _authenticationManager ?? (_authenticationManager = HttpContext.Current.GetOwinContext().Authentication);
            }
        }

        [HttpPost]
		[ActionName("login")]
        [Route("login")]
		public async Task<IHttpActionResult> Login(UserLogin model)
		{
            if (await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, true) == SignInStatus.Success)
            {
                return Ok(GetUserInfo(model.UserName));
            }

             return StatusCode(HttpStatusCode.Unauthorized);
		}

		[Authorize]
		[HttpGet]
		[ActionName("usersession")]
        [Route("usersession")]
		public  IHttpActionResult GetUserSession()
		{
            return Ok(GetUserInfo(User.Identity.Name));
		}

		[HttpPost]
		[ActionName("logout")]
        [Route("logout")]
		public IHttpActionResult Logout()
		{
            AuthenticationManager.SignOut();
			return Ok(new { status = true });
		}


        private AuthInfo GetUserInfo(string userName)
        {
            using (var repository = _securityRepository())
            {
                var user = repository.GetAccount(userName);

                if (user != null)
                {
                    var permissions =
                        user.RoleAssignments.Select(x => x.Role)
                            .SelectMany(x => x.RolePermissions)
                            .Select(x => x.Permission);

                    string fullname = user.UserName;

                    using (var customerRep = _customerRepository())
                    {
                        var contact = customerRep.GetContact(user.MemberId);
                        //Account should allways have associated contact info
                        if (contact != null)
                        {
                            fullname = contact.FullName;
                        }
                    }

                    return new AuthInfo
                    {
                        Login = user.UserName,
                        FullName = fullname,
                        UserType = (RegisterType) user.RegisterType,
                        Permissions = permissions.Select(x => x.PermissionId).Distinct().ToArray()
                    };
                }
            }

            return null;
        }
	}
}
