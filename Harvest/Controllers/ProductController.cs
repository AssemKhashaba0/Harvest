using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Linq.Expressions;

namespace Harvest.Controllers
{

    public class ProductsController : Controller
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;

        public ProductsController(IRepository<Product> productRepository, IRepository<Category> categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public IActionResult UserProducts(int? categoryId, int page = 1)
        {
            int pageSize = 20;
            var productsQuery = categoryId == null
                ? _productRepository.Get(includeProp: new Expression<Func<Product, object>>[] { p => p.Category })
                : _productRepository.Get(includeProp: new Expression<Func<Product, object>>[] { p => p.Category }, expression: p => p.CategoryId == categoryId);

            var paginatedProducts = productsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Categories = _categoryRepository.GetAll();
            ViewBag.SelectedCategory = categoryId;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = Math.Ceiling((double)productsQuery.Count() / pageSize);

            return View(paginatedProducts);
        }
        [Authorize(Roles = "Admin,SubAdmin")]

        public IActionResult AdminProducts()
        {
            var products = _productRepository.Get(includeProp: new Expression<Func<Product, object>>[] { p => p.Category });
            ViewBag.Categories = _categoryRepository.GetAll();
            return View(products);
        }
        [Authorize(Roles = "Admin,SubAdmin")]

        public IActionResult Create()
        {
            ViewBag.Categories = _categoryRepository.GetAll();
            return View();
        }
        [Authorize(Roles = "Admin,SubAdmin")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile? ImageFile)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Categories = _categoryRepository.GetAll();
                return View(product);
            }

            if (ImageFile != null && ImageFile.Length > 0)
            {
                if (!ImageFile.ContentType.StartsWith("image"))
                {
                    ModelState.AddModelError("ImageFile", "يجب أن يكون الملف صورة.");
                    ViewBag.Categories = _categoryRepository.GetAll();
                    return View(product);
                }

                var fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagesProducts", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                product.ImageUrl = "/imagesProducts/" + fileName;
            }

            product.CreatedAt = DateTime.Now;
            product.UpdatedAt = DateTime.Now;

            _productRepository.Create(product);
            _productRepository.Commit();

            TempData["success"] = "تم إنشاء المنتج بنجاح";
            return RedirectToAction(nameof(AdminProducts));
        }
        [Authorize(Roles = "Admin,SubAdmin")]

        public IActionResult Edit(int id)
        {
            var product = _productRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Categories = _categoryRepository.GetAll();
            ViewBag.ImagePreview = product.ImageUrl;
            return View(product);
        }
        [Authorize(Roles = "Admin,SubAdmin")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product, IFormFile? ImageFile)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Categories = _categoryRepository.GetAll();
                return View(product);
            }

            var existingProduct = _productRepository.GetById(product.Id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.OriginCountry = product.OriginCountry;
            existingProduct.PackagingDetails = product.PackagingDetails;
            existingProduct.Price = product.Price;
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.UpdatedAt = DateTime.Now;

            if (ImageFile != null && ImageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingProduct.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                var fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagesProducts", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                existingProduct.ImageUrl = "/imagesProducts/" + fileName;
            }

            _productRepository.Edit(existingProduct);
            _productRepository.Commit();

            TempData["success"] = "تم تعديل المنتج بنجاح";
            return RedirectToAction(nameof(AdminProducts));
        }

        [Authorize(Roles = "Admin,SubAdmin")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var product = _productRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", product.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _productRepository.Delete(product);
            _productRepository.Commit();

            TempData["success"] = "تم حذف المنتج بنجاح";
            return RedirectToAction(nameof(AdminProducts));
        }

        public IActionResult Details(int id)
        {
            var product = _productRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

    }
}
