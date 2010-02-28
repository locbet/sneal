using System.Web.Mvc;
using System.Web.Security;
using Stormwind.Core.Security;

namespace Stormwind.Controllers
{
    [HandleError]
    public class AccountController : Controller
    {
        public AuthenticationService AuthenticationService { get; set; }

        public AccountController(AuthenticationService authenticationService)
        {
            AuthenticationService = authenticationService;
        }

        public ActionResult LogOn()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LogOn(string emailAddress, string password, string returnUrl)
        {
            AuthenticationResult result = AuthenticationService.Authenticate(emailAddress, password);
            if (result.IsError)
            {
                return View();
            }
            FormsAuthentication.SetAuthCookie(result.AuthenticatedUser.EmailAddress, false);
            return SuccessfulLoginActionResult(returnUrl);
        }

        private ActionResult SuccessfulLoginActionResult(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult LogOff()
        {
            AuthenticationService.SignOut();
            FormsAuthentication.SignOut();
            return View("LogOn");
        }
    }
}