using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HealthSolution.Startup))]
namespace HealthSolution
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
