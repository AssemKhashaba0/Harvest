using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Harvest.Controllers
{
    public class MessageController : Controller
    {
        private readonly IRepository<Message> _messageRepository;

        public MessageController(IRepository<Message> messageRepository)
        {
            _messageRepository = messageRepository;
        }
        [Authorize(Roles = "Admin,SubAdmin")]

        // عرض كل الرسائل من الأحدث للأقدم
        public IActionResult Index()
        {
            var messages = _messageRepository.GetAll().OrderByDescending(m => m.CreatedAt).ToList();
            return View(messages);
        }
        [Authorize(Roles = "Admin,SubAdmin")]

        // عرض الرسائل في آخر 24 ساعة
        public IActionResult Last24Hours()
        {
            var last24HoursMessages = _messageRepository.Get(
                expression: m => m.CreatedAt > DateTime.Now.AddDays(-1)  // استخدام تعبير لتصفية الرسائل
            ).OrderByDescending(m => m.CreatedAt).ToList();

            return View(last24HoursMessages);
        }


        // عرض الفورم لإرسال رسالة
        public IActionResult Create()
        {
            return View();
        }

        // إرسال الرسالة
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Message message)
        {
            if (ModelState.IsValid)
            {
                message.CreatedAt = DateTime.Now;
                _messageRepository.Create(message);
                _messageRepository.Commit();
                return RedirectToAction("ThankYou");
            }
            return View(message);
        }

        // عرض صفحة الشكر بعد إرسال الرسالة
        public IActionResult ThankYou()
        {
            return View();
        }
    }
}
