using CafeApplication.Data;
using CafeApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CafeApplication.Pages.Menu
{
    public class NewModel : PageModel
    {
        private readonly CafeDbContext _context;

        public NewModel(CafeDbContext context) => _context = context;

        [BindProperty]
        public MenuItem Menu { get; set; }

    public async Task<ActionResult> OnPost()
        {
            Console.WriteLine("Entered : ");
            Console.WriteLine(Menu);
            if (!ModelState.IsValid) Console.WriteLine("Invalid");
            if (!ModelState.IsValid) return Page();
            Console.WriteLine("Entered : " );
            await _context.Items.AddAsync(Menu);
            await _context.SaveChangesAsync();

            return RedirectToPage("../Index");
        }
    }
}
