using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FLMS.Startup))]
namespace FLMS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
