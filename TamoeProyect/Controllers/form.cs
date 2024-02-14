using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using TamoeProyect.Models;
using TamoeProyect.Services;

namespace TamoeProyect.Controllers
{
    public class form: Controller
    {
        private readonly IRepositorioUser repositorioUser;

        public form(IRepositorioUser repositorioUser)
        {
            this.repositorioUser = repositorioUser;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(User user)
        {
            Console.WriteLine("Controller 1");
            if (!ModelState.IsValid)
            {
                return View(user);
            }
            Console.WriteLine("Controller");
            await repositorioUser.PostUser(user);
            return View();
        }
    }
}
