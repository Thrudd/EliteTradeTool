using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(EDDNService.Startup))]

namespace EDDNService {
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
