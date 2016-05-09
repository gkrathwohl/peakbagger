using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(peaker.Startup))]
namespace peaker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
