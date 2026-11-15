using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using wt_lab8_8razor_postgresql_valkovich.Data;
using wt_lab8_8razor_postgresql_valkovich.Models;

namespace wt_lab8_8razor_postgresql_valkovich.Pages.Products
{
    public class DetailsModel : PageModel
    {
        private readonly AppDbContext _context;
        public DetailsModel(AppDbContext context) => _context = context;

        public Book? Book { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Book = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
            return Book == null ? NotFound() : Page();
        }
    }
}