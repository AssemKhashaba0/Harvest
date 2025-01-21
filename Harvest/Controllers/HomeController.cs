using DataAccess.Repository.IRepository;
using Harvest.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Diagnostics;
using Utility.ViewModel;

namespace Harvest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRepository<Category> categoryRepository;
        private readonly IRepository<HomePage> homePageRepository;

        public HomeController(ILogger<HomeController> logger, IRepository<Category> categoryRepository, IRepository<HomePage> HomePageRepository)
        {
            _logger = logger;
            this.categoryRepository = categoryRepository;
            homePageRepository = HomePageRepository;
        }

        public IActionResult Index()
        {
            var homePage = homePageRepository.GetAll().FirstOrDefault();
            var categories = categoryRepository.GetAll();

            // تأكد أن الـ homePage مش null
            if (homePage != null && !string.IsNullOrEmpty(homePage.logo))
            {
                string homePageLogoUrl = Url.Content(homePage.logo);  
                ViewData["HomePageLogo"] = homePageLogoUrl;
            }

            var viewModel = new HomeIndexViewModel
            {
                HomePage = homePage,
                Categories = categories
            };

            return View(viewModel);
        }



        // صفحة الإضافة
        [Authorize(Roles = "Admin")]

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HomePage homePage, IFormFile logo, IFormFile sliderImage1, IFormFile sliderImage2, IFormFile sliderImage3)
        {
            if (string.IsNullOrEmpty(homePage.CompanyOverview) || string.IsNullOrEmpty(homePage.Vision))
            {
                ModelState.AddModelError("CompanyOverview", "الرجاء ملء كافة الحقول المطلوبة.");
                return View(homePage);
            }

            // رفع الصور فقط إذا كانت موجودة
            if (logo != null && logo.Length > 0)
            {
                var logoFileName = Guid.NewGuid().ToString() + Path.GetExtension(logo.FileName);
                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", logoFileName);

                using (var stream = new FileStream(logoPath, FileMode.Create))
                {
                    await logo.CopyToAsync(stream);
                }

                homePage.logo = "/images/" + logoFileName;
            }

            if (sliderImage1 != null && sliderImage1.Length > 0)
            {
                var slider1FileName = Guid.NewGuid().ToString() + Path.GetExtension(sliderImage1.FileName);
                var slider1Path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", slider1FileName);

                using (var stream = new FileStream(slider1Path, FileMode.Create))
                {
                    await sliderImage1.CopyToAsync(stream);
                }

                homePage.SliderImage1 = "/images/" + slider1FileName;
            }

            if (sliderImage2 != null && sliderImage2.Length > 0)
            {
                var slider2FileName = Guid.NewGuid().ToString() + Path.GetExtension(sliderImage2.FileName);
                var slider2Path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", slider2FileName);

                using (var stream = new FileStream(slider2Path, FileMode.Create))
                {
                    await sliderImage2.CopyToAsync(stream);
                }

                homePage.SliderImage2 = "/images/" + slider2FileName;
            }

            if (sliderImage3 != null && sliderImage3.Length > 0)
            {
                var slider3FileName = Guid.NewGuid().ToString() + Path.GetExtension(sliderImage3.FileName);
                var slider3Path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", slider3FileName);

                using (var stream = new FileStream(slider3Path, FileMode.Create))
                {
                    await sliderImage3.CopyToAsync(stream);
                }

                homePage.SliderImage3 = "/images/" + slider3FileName;
            }

           

            homePageRepository.Create(homePage);
            homePageRepository.Commit();

            TempData["success"] = "تم إضافة الصفحة الرئيسية بنجاح.";
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin")]

        // صفحة التعديل
        public IActionResult Edit(int id)
        {
            var homePage = homePageRepository.GetById(id);
            if (homePage == null)
            {
                return NotFound();
            }
            return View(homePage);
        }
        [Authorize(Roles = "Admin")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(HomePage homePage, IFormFile? logo, IFormFile? sliderImage1, IFormFile? sliderImage2, IFormFile? sliderImage3)
        {
            if (string.IsNullOrEmpty(homePage.CompanyOverview) || string.IsNullOrEmpty(homePage.Vision))
            {
                ModelState.AddModelError("CompanyOverview", "الرجاء ملء كافة الحقول المطلوبة.");
                return View(homePage);
            }

            var existingHomePage = homePageRepository.GetById(homePage.Id);
            if (existingHomePage == null)
            {
                return NotFound();
            }

            existingHomePage.CompanyOverview = homePage.CompanyOverview;
            existingHomePage.Vision = homePage.Vision;
            existingHomePage.FacebookLink = homePage.FacebookLink;
            existingHomePage.YouTubeLink = homePage.YouTubeLink;
            existingHomePage.WhatsAppNumber = homePage.WhatsAppNumber;
            existingHomePage.ContactNumber = homePage.ContactNumber;
            existingHomePage.Email = homePage.Email;

            // رفع الصور فقط إذا كانت موجودة
            if (logo != null && logo.Length > 0)
            {
                var oldLogoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingHomePage.logo.TrimStart('/'));
                if (System.IO.File.Exists(oldLogoPath))
                {
                    System.IO.File.Delete(oldLogoPath);
                }

                var logoFileName = Guid.NewGuid().ToString() + Path.GetExtension(logo.FileName);
                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", logoFileName);

                using (var stream = new FileStream(logoPath, FileMode.Create))
                {
                    await logo.CopyToAsync(stream);
                }

                existingHomePage.logo = "/images/" + logoFileName;
            }

            if (sliderImage1 != null && sliderImage1.Length > 0)
            {
                var oldSlider1Path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingHomePage.SliderImage1.TrimStart('/'));
                if (System.IO.File.Exists(oldSlider1Path))
                {
                    System.IO.File.Delete(oldSlider1Path);
                }

                var slider1FileName = Guid.NewGuid().ToString() + Path.GetExtension(sliderImage1.FileName);
                var slider1Path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", slider1FileName);

                using (var stream = new FileStream(slider1Path, FileMode.Create))
                {
                    await sliderImage1.CopyToAsync(stream);
                }

                existingHomePage.SliderImage1 = "/images/" + slider1FileName;
            }

            if (sliderImage2 != null && sliderImage2.Length > 0)
            {
                var oldSlider2Path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingHomePage.SliderImage2.TrimStart('/'));
                if (System.IO.File.Exists(oldSlider2Path))
                {
                    System.IO.File.Delete(oldSlider2Path);
                }

                var slider2FileName = Guid.NewGuid().ToString() + Path.GetExtension(sliderImage2.FileName);
                var slider2Path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", slider2FileName);

                using (var stream = new FileStream(slider2Path, FileMode.Create))
                {
                    await sliderImage2.CopyToAsync(stream);
                }

                existingHomePage.SliderImage2 = "/images/" + slider2FileName;
            }

            if (sliderImage3 != null && sliderImage3.Length > 0)
            {
                var oldSlider3Path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingHomePage.SliderImage3.TrimStart('/'));
                if (System.IO.File.Exists(oldSlider3Path))
                {
                    System.IO.File.Delete(oldSlider3Path);
                }

                var slider3FileName = Guid.NewGuid().ToString() + Path.GetExtension(sliderImage3.FileName);
                var slider3Path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", slider3FileName);

                using (var stream = new FileStream(slider3Path, FileMode.Create))
                {
                    await sliderImage3.CopyToAsync(stream);
                }

                existingHomePage.SliderImage3 = "/images/" + slider3FileName;
            }

            homePageRepository.Edit(existingHomePage);
            homePageRepository.Commit();

            TempData["success"] = "تم تحديث الصفحة الرئيسية بنجاح.";
            return RedirectToAction(nameof(Index));
        }
    }
}
