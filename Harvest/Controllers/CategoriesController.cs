using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.IO;

namespace Harvest.Controllers
{
    [Authorize(Roles = "Admin,SubAdmin")]

    public class CategoriesController : Controller
    {
        private readonly IRepository<Category> _categoryRepository;

        public CategoriesController(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public IActionResult Index()
        {
            var categories = _categoryRepository.GetAll();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category, IFormFile ImgUrl)
        {
            if (string.IsNullOrEmpty(category.Name))
            {
                ModelState.AddModelError("Name", "اسم القسم مطلوب.");
                return View(category);
            }

            // رفع الصورة فقط إذا كانت موجودة
            if (ImgUrl != null && ImgUrl.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImgUrl.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagesCategory", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await ImgUrl.CopyToAsync(stream);
                }

                category.ImageUrl = "/imagesCategory/" + fileName;
            }

            category.CreatedAt = DateTime.Now;
            category.UpdatedAt = DateTime.Now;

            _categoryRepository.Create(category);
            _categoryRepository.Commit();

            TempData["success"] = "Category created successfully";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var category = _categoryRepository.GetById(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category, IFormFile? ImgUrl)
        {
            if (string.IsNullOrEmpty(category.Name))
            {
                ModelState.AddModelError("Name", "اسم القسم مطلوب.");
                return View(category);
            }

            var existingCategory = _categoryRepository.GetById(category.Id);
            if (existingCategory == null)
            {
                return NotFound();
            }

            existingCategory.Name = category.Name;
            existingCategory.UpdatedAt = DateTime.Now;

            // رفع الصورة فقط إذا كانت موجودة
            if (ImgUrl != null && ImgUrl.Length > 0)
            {
                // حذف الصورة القديمة إذا كانت موجودة
                if (!string.IsNullOrEmpty(existingCategory.ImageUrl))
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingCategory.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImgUrl.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagesCategory", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await ImgUrl.CopyToAsync(stream);
                }

                existingCategory.ImageUrl = "/imagesCategory/" + fileName;
            }

            _categoryRepository.Edit(existingCategory);
            _categoryRepository.Commit();

            TempData["success"] = "تم تحديث القسم بنجاح";
            return RedirectToAction(nameof(Index));
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var category = _categoryRepository.GetById(id);
            if (category == null)
            {
                return NotFound();
            }

            // حذف الصورة من النظام إذا كانت موجودة
            if (!string.IsNullOrEmpty(category.ImageUrl))
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", category.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _categoryRepository.Delete(category);
            _categoryRepository.Commit();

            TempData["success"] = "تم حذف القسم بنجاح";
            return RedirectToAction(nameof(Index));
        }
    }
}
