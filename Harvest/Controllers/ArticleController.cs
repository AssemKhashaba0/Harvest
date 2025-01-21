using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Harvest.Controllers
{
    public class ArticleController : Controller
    {
        private readonly IRepository<BlogPost> _articleRepo;

        public ArticleController(IRepository<BlogPost> articleRepo)
        {
            _articleRepo = articleRepo;
        }

        // صفحة إضافة المقالة
        [Authorize(Roles = "Admin,SubAdmin")]

        public IActionResult Create()
        {
            return View(new BlogPost());
        }
        [Authorize(Roles = "Admin,SubAdmin")]

        [HttpPost]
        public IActionResult Create(BlogPost article)
        {
            if (ModelState.IsValid)
            {
                article.CreatedAt = DateTime.Now;
                article.UpdatedAt = DateTime.Now;
                _articleRepo.Create(article);
                _articleRepo.Commit();
                return RedirectToAction(nameof(Index));
            }
            return View(article);
        }

        // صفحة عرض جميع المقالات
        public IActionResult Index()
        {
            var articles = _articleRepo.GetAll().OrderByDescending(a => a.CreatedAt).ToList();
            return View(articles);
        }

        // صفحة عرض تفاصيل المقالة
        public IActionResult Details(int id)
        {
            var article = _articleRepo.GetById(id);
            if (article == null)
            {
                return NotFound();
            }
            return View(article);
        }

        [Authorize(Roles = "Admin,SubAdmin")]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var article = _articleRepo.GetById(id);
            if (article == null)
            {
                return NotFound();
            }

            _articleRepo.Delete(article);
            _articleRepo.Commit();
            return RedirectToAction(nameof(Index));
        }
    }
}
