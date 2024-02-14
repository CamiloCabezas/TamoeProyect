using Microsoft.AspNetCore.Mvc;
using TamoeProyect.Models;
using TamoeProyect.Services;

namespace TamoeProyect.Controllers
{
    public class ProductController : Controller
    {
        private readonly IRepositorioProducts repositorioProducts;

        public ProductController(IRepositorioProducts repositorioProducts)
        {
            this.repositorioProducts = repositorioProducts;
        }

        public async Task<IActionResult> Index()
        {
            var ListProducts = await repositorioProducts.GetAllProducts();
            return View(ListProducts);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            await repositorioProducts.PostProduct(modelo);

            return RedirectToAction("Index");
        }
    }
}
