using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using wt_lab12_12auth_validation_valkovich.Data;
using wt_lab12_12auth_validation_valkovich.Models;

namespace wt_lab12_12auth_validation_valkovich.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;
        public IndexModel(AppDbContext context) => _context = context;

        public List<Book> Books { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        public async Task OnGetAsync()
        {
            var query = _context.Books.AsQueryable();
            if (!string.IsNullOrWhiteSpace(Search))
                query = query.Where(b => b.Title.Contains(Search));

            Books = await query.OrderBy(b => b.Id).ToListAsync();
        }
    }
}