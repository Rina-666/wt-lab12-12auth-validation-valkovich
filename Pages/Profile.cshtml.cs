using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using wt_lab12_12auth_validation_valkovich.Data;

namespace wt_lab12_12auth_validation_valkovich.Pages
{
    public class ProfileModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp", ".gif"];
        private const long MaxFileSizeBytes = 2 * 1024 * 1024; // 2 МБ

        public ProfileModel(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public int UserId { get; private set; }
        public string? UserLogin { get; private set; }
        public string RoleTitle { get; private set; } = "Студент / Пользователь";
        public string? AvatarPath { get; private set; }

        [BindProperty]
        public IFormFile? AvatarFile { get; set; }

        [TempData]
        public string? StatusMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var sessionLogin = HttpContext.Session.GetString("UserLogin");
            if (string.IsNullOrWhiteSpace(sessionLogin))
            {
                return RedirectToPage("/Login");
            }

            await LoadUserDataAsync(sessionLogin);
            return Page();
        }

        public async Task<IActionResult> OnPostUploadAvatarAsync()
        {
            var sessionLogin = HttpContext.Session.GetString("UserLogin");
            if (string.IsNullOrWhiteSpace(sessionLogin))
            {
                return RedirectToPage("/Login");
            }

            await LoadUserDataAsync(sessionLogin);

            if (AvatarFile == null || AvatarFile.Length == 0)
            {
                ErrorMessage = "Выберите файл изображения перед загрузкой.";
                return RedirectToPage("/Profile");
            }

            if (AvatarFile.Length > MaxFileSizeBytes)
            {
                ErrorMessage = "Размер фото не должен превышать 2 МБ.";
                return RedirectToPage("/Profile");
            }

            var extension = Path.GetExtension(AvatarFile.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
            {
                ErrorMessage = "Разрешены только форматы JPG, JPEG, PNG, WEBP и GIF.";
                return RedirectToPage("/Profile");
            }

            var avatarsFolder = Path.Combine(_environment.WebRootPath, "uploads", "avatars");
            Directory.CreateDirectory(avatarsFolder);

            var safeLoginPart = new string(sessionLogin
                .Where(ch => char.IsLetterOrDigit(ch) || ch == '_' || ch == '-')
                .ToArray());
            if (string.IsNullOrWhiteSpace(safeLoginPart))
            {
                safeLoginPart = "user";
            }

            var fileName = $"{safeLoginPart}_{Guid.NewGuid():N}{extension}";
            var fullPath = Path.Combine(avatarsFolder, fileName);

            await using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await AvatarFile.CopyToAsync(stream);
            }

            var relativePath = $"/uploads/avatars/{fileName}";
            HttpContext.Session.SetString("UserAvatar", relativePath);
            StatusMessage = "Фото профиля успешно обновлено.";

            return RedirectToPage("/Profile");
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }

        private async Task LoadUserDataAsync(string login)
        {
            UserLogin = login;
            AvatarPath = HttpContext.Session.GetString("UserAvatar");

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Login == login);

            if (user != null)
            {
                UserId = user.Id;
            }
        }
    }
}