using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(JsonFileCreatorFromExcel.Startup))]
namespace JsonFileCreatorFromExcel
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
           
        }
    }
}
