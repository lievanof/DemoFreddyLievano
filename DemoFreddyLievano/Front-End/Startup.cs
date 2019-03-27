using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Frontend.Identity.Startup))]
namespace Frontend.Identity
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
