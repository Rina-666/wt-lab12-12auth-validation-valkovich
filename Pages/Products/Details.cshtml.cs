using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using wt_lab4_4mvc_valkovich.Models;
using wt_lab4_4mvc_valkovich.Data;

namespace wt_lab4_4mvc_valkovich.Pages.Products
{
    public class DetailsModel : PageModel
    {
        public Book? Book { get; set; }

        // 1. Загрузка страницы (GET)
        public IActionResult OnGet(int id)
        {
            Book = BookStore.Books.FirstOrDefault(b => b.Id == id);
            if (Book == null) return NotFound();
            return Page();
        }

        // 2. Сохранение изменений (POST) - добавьте этот метод!
        public IActionResult OnPost(int id, bool isAvailable)
        {
            var book = BookStore.Books.FirstOrDefault(b => b.Id == id);
            if (book == null) return NotFound();

            // Меняем статус на тот, что пришел из формы
            book.IsAvailable = isAvailable;

            // Возвращаемся на ту же страницу
            return RedirectToPage(new { id = id });
        }
    }
}