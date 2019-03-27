using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace Frontend.Authorization
{
    public class Autorize: AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!AuthorizeCore(filterContext.HttpContext))
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { area = "", controller = "Account", action = "Login" }));
            }
            //base.HandleUnauthorizedRequest(filterContext);
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool expired = true;

            HttpCookie cookie = httpContext.Request.Cookies["TTCookieAuth"];

            if(cookie != null)
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                expired = ticket.Expired;
            }

            if (expired)
            {
                var authToken = httpContext.Request.Headers["Authorization"];
                if(!string.IsNullOrEmpty(authToken))
                {
                    expired = false;
                }
            }

            return !expired;
        }
    }
}