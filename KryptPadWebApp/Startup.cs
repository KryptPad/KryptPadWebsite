using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(KryptPadWebApp.Startup))]
namespace KryptPadWebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
