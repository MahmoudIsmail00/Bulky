
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repositories.IRepositories;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace BulkyApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IToastNotification _toastNotification;
        public CategoryController(IUnitOfWork unitOfWork, IToastNotification toastNotification)
        {
            _unitOfWork = unitOfWork;
            _toastNotification = toastNotification;
        }
        public IActionResult Index()
        {
            var CategoryList = _unitOfWork.Category.GetAll();
            return View(CategoryList);
        }
        public IActionResult Create()
        {
            return View("CategoryForm", new Category());
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
                return View("CategoryForm", obj);

            _unitOfWork.Category.Add(obj);
            _unitOfWork.Save();
            _toastNotification.AddSuccessToastMessage("Category Created Successfully");
            return RedirectToAction(nameof(Index));

        }
        public IActionResult Edit(int? id)
        {

            if (id == null)
                return BadRequest();

            var model = _unitOfWork.Category.Get(m => m.Id == id);

            if (model == null)
                return NotFound();

            return View("CategoryForm", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (!ModelState.IsValid)
                return View("CategoryForm", category);

            //var categoryFromDb = _categoryRepo.Categories.FirstOrDefault(c => c.Id == category.Id);
            //if (categoryFromDb == null)
            //    return NotFound();

            _unitOfWork.Category.Update(category);
            _unitOfWork.Save();
            _toastNotification.AddSuccessToastMessage("Category Updated Successfully");
            return RedirectToAction(nameof(Index));

        }
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return BadRequest();

            var category = _unitOfWork.Category.Get(m => m.Id == id);

            if (category == null)
                return NotFound();

            _unitOfWork.Category.Remove(category);
            _unitOfWork.Save();

            return Ok();
        }
    }
}
