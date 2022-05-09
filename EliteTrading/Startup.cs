using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EliteTrading.Startup))]
namespace EliteTrading
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
