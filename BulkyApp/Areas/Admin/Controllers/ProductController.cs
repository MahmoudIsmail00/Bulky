using Bulky.DataAccess.Repositories.IRepositories;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NToastNotify;

namespace BulkyApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IToastNotification _toastNotification;
        //To Access the wwwroot folder and deal with images
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, IToastNotification toastNotification)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _toastNotification = toastNotification;
        }
        public IActionResult Index()
        {
            var ProductList = _unitOfWork.Product.GetAll(includeProperties:"Category");
          
            return View(ProductList);
        }
        public IActionResult Upsert(int? id)
        {
            /*
             *  // مينفعش احط ليست من الكاتيجوريز في ليست من السيليكت ايتم
             // هنستخدم حاجه اسمها Projection   .Select(u => new SelectListItem)
             // we can send it using viewbag , viewData , tempData , viewModel
             //ViewBag.Categorylist = CategoryList;
             */
            ProductVM productVM = new ProductVM {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem{Text = u.Name,Value = u.Id.ToString()}),
                Product = new Product()};

            if (id == null || id == 0)
                return View("ProductForm", productVM);
            else
            {
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
                return View("ProductForm", productVM);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            productVM.CategoryList = _unitOfWork.Category
                .GetAll().Select(u => new SelectListItem{Text = u.Name,Value = u.Id.ToString()});

            if (!ModelState.IsValid)
                return View("ProductForm", productVM);

            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if(file != null)
            {
                //file name
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                // location
                string productPath = Path.Combine(wwwRootPath,@"images\product");

                if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                {
                    var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                        System.IO.File.Delete(oldImagePath);
                }

                using (var fileStream = new FileStream(Path.Combine(productPath, fileName),FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
                productVM.Product.ImageUrl = @"\images\product\" + fileName ;
            }

            if(productVM.Product.Id != 0)
            {
                _unitOfWork.Product.Update(productVM.Product);
                _toastNotification.AddSuccessToastMessage("product Updated Successfully");
            }
            else
            {
                _unitOfWork.Product.Add(productVM.Product);
                _toastNotification.AddSuccessToastMessage("product Created Successfully");
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));

        }
        //public IActionResult Edit(int? id)
        //{

        //    if (id == null)
        //        return BadRequest();

        //    var model = _unitOfWork.Product.Get(m => m.Id == id);

        //    if (model == null)
        //        return NotFound();

        //    ProductVM productVM = new ProductVM
        //    {
        //        CategoryList = _unitOfWork.Category
        //        .GetAll().Select(u => new SelectListItem
        //        {
        //            Text = u.Name,
        //            Value = u.Id.ToString()
        //        }),

        //        Product = model
        //    };
        //    return View("ProductForm", productVM);
        //}
        /*  edit action
         *  [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductVM productVM)
        {
            // لو عملت البارمتر نفس اسم الاوبجيكت اللي راجع وليكن product
            // هيحصل ايرور وال model binder مش هيعرف يعمل بايند للقيم
            productVM.CategoryList = _unitOfWork.Category
                .GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
            if (!ModelState.IsValid)
                return View("ProductForm", productVM);

            

            _unitOfWork.Product.Update(productVM.Product);
            _unitOfWork.Save();
            _toastNotification.AddSuccessToastMessage("product Updated Successfully");
            return RedirectToAction(nameof(Index));

        }
         */

        public IActionResult Delete(int? id)
        {
            if (id == null)
                return BadRequest();

            var product = _unitOfWork.Product.Get(m => m.Id == id);

            if (product == null)
                return NotFound();
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            var oldImagePath = Path.Combine(wwwRootPath, product.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagePath))
                System.IO.File.Delete(oldImagePath);
            _unitOfWork.Product.Remove(product);
            _unitOfWork.Save();

            return Ok();
        }
        #region APICALLS

        [HttpGet]
        public IActionResult GetData()
        {
            List<Product> ProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = ProductList });
        }
        #endregion
    }
}
