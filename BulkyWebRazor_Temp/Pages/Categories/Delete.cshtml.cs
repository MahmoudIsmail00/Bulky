using BulkyWebRazor_Temp.Data;
using BulkyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;

namespace BulkyWebRazor_Temp.Pages.Categories
{
    [BindProperties]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;

        public Category Category { get; set; }
        public DeleteModel(ApplicationDbContext context,IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }

        public void OnGet(int? id)
        {
            if (id != null & id != 0)
                Category = _context.Categories.Find(id);

        }
        public IActionResult OnPost()
        {
            var obj = _context.Categories.Find(Category.Id);
            if (obj is null)
                return NotFound();

       
            _context.Categories.Remove(obj);
            _context.SaveChanges();
            _toastNotification.AddSuccessToastMessage("Category Deleted Successfully");
            return RedirectToPage("Index");
            
            
        }
        
    }
}
