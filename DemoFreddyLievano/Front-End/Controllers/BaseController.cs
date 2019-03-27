using Demo.Model;
using Frontend.Utils;
using Newtonsoft.Json;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Frontend.Controllers
{
    public class BaseController : Controller
    {
        private User currentUser;

        protected User CurrentUser
        {
            get
            {
                return currentUser ?? GetCurrentUser();
            }
        }

        private User GetCurrentUser()
        {
            HttpCookie cookie = HttpContext.Request.Cookies["TTCookieAuth"];

            if (cookie != null)
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                if (ticket != null)
                {
                    currentUser = JsonConvert.DeserializeObject<User>(ticket.UserData);
                }
            }

            return currentUser;
        }

        public void Attention(string message)
        {
            if (TempData.ContainsKey(Alerts.ATTENTION) == false)
            {
                TempData.Add(Alerts.ATTENTION, message);
            }
        }

        public void Success(string message)
        {
            if (TempData.ContainsKey(Alerts.SUCCESS) == false)
            {
                TempData.Add(Alerts.SUCCESS, message);
            }
        }

        public void Information(string message)
        {
            if (TempData.ContainsKey(Alerts.INFORMATION) == false)
            {
                TempData.Add(Alerts.INFORMATION, message);
            }
        }

        public void Error(string message)
        {
            if (TempData.ContainsKey(Alerts.ERROR) == false)
            {
                TempData.Add(Alerts.ERROR, message);
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.UserName = CurrentUser != null ? CurrentUser.FullName : "";
            base.OnActionExecuting(filterContext);
        }
    }
}