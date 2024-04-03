using BulkyApp.Data;
using BulkyApp.Models;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace BulkyApp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;
        public CategoryController(ApplicationDbContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }
        public IActionResult Index()
        {
            var CategoryList = _context.Categories.ToList();
            return View(CategoryList);
        }
        public IActionResult Create()
        {
            return View("CategoryForm",new Category());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category obj)
        {

            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "Display order cannot exactly match the name");
            }
            if (obj.Name is not null && obj.Name.ToLower() == "test")
            {
                ModelState.AddModelError("", "Test is an invalid value");
            }

            if (!ModelState.IsValid)
                return View("CategoryForm",obj);

            _context.Categories.Add(obj);
            _context.SaveChanges();
            _toastNotification.AddSuccessToastMessage("Category Created Successfully");
            return RedirectToAction(nameof(Index));
            
        }
        public IActionResult Edit(int? id)
        {

            if (id == null)
                return BadRequest();

            var model = _context.Categories.Find(id);

            if (model == null)
                return NotFound();

            return View("CategoryForm",model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (!ModelState.IsValid)
                return View("CategoryForm",category);

            //var categoryFromDb = _context.Categories.FirstOrDefault(c => c.Id == category.Id);

            //if (categoryFromDb == null)
            //    return NotFound();

            _context.Categories.Update(category);
            _context.SaveChanges();
            _toastNotification.AddSuccessToastMessage("Category Updated Successfully");
            return RedirectToAction(nameof(Index));

        }
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return BadRequest();

            var category =  _context.Categories.Find(id);

            if (category == null)
                return NotFound();

            _context.Categories.Remove(category);
            _context.SaveChanges();

            return Ok();
        }
    }
}
