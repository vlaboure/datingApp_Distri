using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace DattingApp.api.Controllers
{
    public class Fallback: Controller
    {
        //controller de redirection vers index.html
        public IActionResult Index()
        {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(),"wwwroot",
                "index.html"),"text/HTML");
        }
    }
}