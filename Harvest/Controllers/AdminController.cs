using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Harvest.Controllers
{
    [Authorize(Roles = "Admin")]

    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // عرض صفحة إضافة مستخدم جديد
        public IActionResult CreateUser()
        {
            return View();
        }

        // معالجة إضافة مستخدم جديد
        [HttpPost]
        public async Task<IActionResult> CreateUser(string username, string password)
        {
            var user = new IdentityUser { UserName = username };
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                // تعيين المستخدم الجديد كـ Admin (أو دور آخر)
                await _userManager.AddToRoleAsync(user, "SubAdmin");
                return RedirectToAction("Index", "Home"); // يمكنك تغيير الصفحة التي يتم تحويل المستخدم إليها
            }

            // إذا كان هناك خطأ
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View();
        }

        // عرض جميع المستخدمين
        public async Task<IActionResult> ListUsers()
        {
            var users = _userManager.Users.ToList(); // جلب جميع المستخدمين من قاعدة البيانات
            return View(users); // إرسال المستخدمين إلى الـ View
        }
        // حذف مستخدم
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(); // المستخدم غير موجود
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ListUsers)); 
            }

            TempData["Error"] = "Failed to delete user. Please try again.";
            return RedirectToAction(nameof(ListUsers));
        }

    }
}
