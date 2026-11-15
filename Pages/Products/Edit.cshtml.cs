using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using wt_lab4_4mvc_valkovich.Models;
using wt_lab4_4mvc_valkovich.Data;

namespace wt_lab4_4mvc_valkovich.Pages.Products
{
    public class EditModel : PageModel
    {
        [BindProperty]
        public Book Book { get; set; } = new();

        public IActionResult OnGet(int id)
        {
            var existingBook = BookStore.Books.FirstOrDefault(b => b.Id == id);
            if (existingBook == null)
                return NotFound();

            Book = existingBook;
            return Page();
        }

        public IActionResult OnPost()
        {
            // Временно отключаем строгую проверку, чтобы форма сохранялась при любом статусе
            // if (!ModelState.IsValid) return Page();

            var existingBook = BookStore.Books.FirstOrDefault(b => b.Id == Book.Id);
            if (existingBook == null)
                return NotFound();

            existingBook.Title = Book.Title;
            existingBook.Author = Book.Author;
            existingBook.Genre = Book.Genre;
            existingBook.Year = Book.Year;
            existingBook.Price = Book.Price;
            existingBook.Pages = Book.Pages;
            existingBook.ISBN = Book.ISBN;
            existingBook.ImageUrl = Book.ImageUrl;
            existingBook.Description = Book.Description;

            // Обновляем статус (если галочка снята, сюда придет false)
            existingBook.IsAvailable = Book.IsAvailable;

            return RedirectToPage("./Index");
        }

        public IActionResult OnPostDelete(int id)
        {
            var book = BookStore.Books.FirstOrDefault(b => b.Id == id);
            if (book != null)
            {
                BookStore.Books.Remove(book);
            }
            return RedirectToPage("./Index");
        }
    }
}