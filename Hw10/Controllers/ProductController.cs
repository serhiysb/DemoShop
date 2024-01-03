using Hw10.Data;
using Hw10.Infrastructure;
using Hw10.Models;
using Hw10.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Hw10.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1)
        {

            List<ProductListViewModel> productListViewModelList = new List<ProductListViewModel>();
            var productList = await _context.Products.Include(p=>p.Category).ToListAsync();

            if (productList != null)
            {
                int pageSize = 3;
                var count = productList.Count();
                var items = productList.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                PaginationViewModel pagination = new PaginationViewModel(count, page, pageSize);

                foreach (var item in items)
                {
                    ProductListViewModel productListViewModel = new
                   ProductListViewModel();
                    productListViewModel.Id = item.Id;
                    productListViewModel.Name = item.Name;
                    productListViewModel.Description = item.Description;
                    productListViewModel.Color = item.Color;
                    productListViewModel.Price = item.Price;
                    productListViewModel.CategoryId = item.CategoryId;
                    productListViewModel.Image = item.Image;
                    productListViewModel.CategoryName = await _context.Categories.Where(c =>
                   c.Id == item.CategoryId).Select(c => c.Name).FirstOrDefaultAsync();
                    productListViewModelList.Add(productListViewModel);
                }

                IndexViewModel<ProductListViewModel> index = new IndexViewModel<ProductListViewModel>(productListViewModelList, pagination);
                return View(index);
            }
            return NotFound();
        }

        public async Task<IActionResult>Create()
        {
            ProductViewModel productCreateViewModel = new ProductViewModel();
            productCreateViewModel.Category =
             await  _context.Categories.Select(c => new SelectListItem()
           {
               Text = c.Name,
               Value = c.Id.ToString()
           }).ToListAsync();
            return View(productCreateViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel productCreateViewModel)
        {
            productCreateViewModel.Category = (IEnumerable<SelectListItem>)_context.Categories.Select(c => new SelectListItem()
            {
                Text = c.Name,
                Value = c.Id.ToString()
            });
            var product = new Product()
            {
                Name = productCreateViewModel.Name,
                Description = productCreateViewModel.Description,
                Price = productCreateViewModel.Price,
                Color = productCreateViewModel.Color,
                CategoryId = productCreateViewModel.CategoryId,
                Image = productCreateViewModel.Image
            };
            ModelState.Remove("Category");
            if (ModelState.IsValid)
            {
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMsg"] = "Product (" + product.Name + ") added successfully.";
            return RedirectToAction("Index");
            }
            return View(productCreateViewModel);
        }

        public async Task <IActionResult> Edit(int? id)
        {
            var productToEdit = await _context.Products.FindAsync(id);
            if (productToEdit != null)
            {
                var productViewModel = new ProductViewModel()
                {
                    Id = productToEdit.Id,
                    Name = productToEdit.Name,
                    Description = productToEdit.Description,
                    Price = productToEdit.Price,
                    CategoryId = productToEdit.CategoryId,
                    Color = productToEdit.Color,
                    Image = productToEdit.Image,
                    Category = (IEnumerable<SelectListItem>)_context.Categories
               .Select(c => new SelectListItem()
               {
                   Text = c.Name,
                   Value = c.Id.ToString()
               })
                };
                return View(productViewModel);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductViewModel productViewModel)
        {
            productViewModel.Category =
           (IEnumerable<SelectListItem>)_context.Categories.Select(c => new SelectListItem()
           {
               Text = c.Name,
               Value = c.Id.ToString()
           });
            var product = new Product()
            {
                Id = productViewModel.Id,
                Name = productViewModel.Name,
                Description = productViewModel.Description,
                Price = productViewModel.Price,
                Color = productViewModel.Color,
                CategoryId = productViewModel.CategoryId,
                Image = productViewModel.Image
            };
            ModelState.Remove("Category");
            if (ModelState.IsValid)
            {
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMsg"] = "Product (" + product.Name + ") updated successfully!";
            return RedirectToAction("Index");
            }
            return View(productViewModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            var productToEdit = await _context.Products.FindAsync(id);
            if (productToEdit != null)
            {
                var productViewModel = new ProductViewModel()
                {
                    Id = productToEdit.Id,
                    Name = productToEdit.Name,
                    Description = productToEdit.Description,
                    Price = productToEdit.Price,
                    CategoryId = productToEdit.CategoryId,
                    Color = productToEdit.Color,
                    Image = productToEdit.Image,
                    Category = (IEnumerable<SelectListItem>)_context.Categories
               .Select(c => new SelectListItem()
               {
                   Text = c.Name,
                   Value = c.Id.ToString()
               })
                };
                return View(productViewModel);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int? id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            TempData["SuccessMsg"] = "Product (" + product.Name + ") deleted successfully.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Buy(int? id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            

            if (!HttpContext.Session.Keys.Contains("Products"))
            {
                List<Product> products = new List<Product>();

                products.Add(product);
                HttpContext.Session.Set<List<Product>>("Products", products);
            }
            else
            {
                var sessionProducts = HttpContext.Session.Get<List<Product>>("Products");
                sessionProducts?.Add(product);
                HttpContext.Session.Set<List<Product>>("Products", sessionProducts);
            }


            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Cart()
        {
            if(HttpContext.Session.Keys.Contains("Products"))
            {
                var products = HttpContext.Session.Get<List<Product>>("Products");
                return View(products);
            }
            return View(new List<Product>());
        }

        public IActionResult DeleteOrder(int? id)
        {
            var products = HttpContext.Session.Get<List<Product>>("Products");
            var product = products.FirstOrDefault(p => p.Id == id);
            var index = products.IndexOf(product);
            products.RemoveAt(index);
            HttpContext.Session.Set<List<Product>>("Products", products);
            return RedirectToAction("Cart");
        }
    }
}
