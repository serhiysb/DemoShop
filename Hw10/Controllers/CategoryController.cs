using Hw10.Data;
using Hw10.Models;
using Hw10.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hw10.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            IEnumerable<Category> categories = await _context.Categories.ToListAsync();

            if(categories!=null)
            {
                int pageSize = 3;
                var count = categories.Count();
                var items = categories.Skip((page-1)*pageSize).Take(pageSize).ToList();
                PaginationViewModel pagination = new PaginationViewModel(count, page, pageSize);
                IndexViewModel<Category> index = new IndexViewModel<Category>(items, pagination);
                return View(index);
            }
            return NotFound();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();
                TempData["SuccessMsg"] = $"Категорія {category.Name} додана успішно";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public async Task<IActionResult> Edit(int? categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if(category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Update(category);
                await _context.SaveChangesAsync();
                TempData["SuccessMsg"] = $"Category {category.Name} added";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            var category = await _context.Categories.FindAsync(id);
            if( category == null )
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int? id)
        {
            var category = await _context.Categories.FindAsync(id);
            if(category == null)
            {
                return NotFound();
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            TempData["SuccessMsg"] = $"Category {category.Name} deleted";
            return RedirectToAction("Index");
        }


    }
}
