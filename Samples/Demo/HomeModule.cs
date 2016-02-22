using Nancy;

namespace Demo
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/"] = _ => View["home"];
        }
    }
}