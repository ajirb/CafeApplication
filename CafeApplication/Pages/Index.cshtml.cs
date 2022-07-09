using CafeApplication.Data;
using CafeApplication.Models;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CafeApplication.Pages
{
    public class IndexModel : PageModel
    {
        private readonly CafeDbContext _context;

        public IndexModel (CafeDbContext context) => _context = context;
        public IEnumerable<MenuItem> Items { get; set; } = Enumerable.Empty<MenuItem>();
        public Dictionary<int,List<MenuItem>> menu { get; set; } = new Dictionary<int, List<MenuItem>>();
        public string errorMsg { get; set; }
        public string filterStr { get; set; } = "";

        public void enumToDictionary(IEnumerable<MenuItem> Items) {
            foreach (var item in Items) {
                List<MenuItem> list;
                int dayDiff = (int)((DateTime.Now - item.MenuDate).TotalDays/7);
                if (menu.ContainsKey(dayDiff)) list = menu[dayDiff];
                else list = new List<MenuItem>();
                list.Add(item);
                menu[dayDiff] = list;
            }
            printMenu();
        }
        public async void OnGet()
        {
            Console.WriteLine("OnGet called");
            errorMsg = "";filterStr = "";
            Items = await _context.Items
                .ToListAsync();
            enumToDictionary(Items);
        }

        public async void OnPost(IFormFile file)
        {
            errorMsg = ""; filterStr = "";
            Items = await _context.Items
                .ToListAsync();
            if (file == null) {
                Console.WriteLine("Empty File");
                errorMsg = "Empty File";
                enumToDictionary(Items);
                return;
            }
            if (!Path.GetExtension(file.FileName).Equals(".xlsx")) {
                errorMsg = "Invalid file type";
                Console.WriteLine("Invalid file type");
                enumToDictionary(Items);
                return;
            }
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                stream.Position = 0;
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    int rowCount = 0;
                    while (reader.Read()) //Each row of the file
                    {
                        if (rowCount++ == 0) {
                            if (reader.FieldCount != 5) {
                                Console.WriteLine(reader.FieldCount);
                                enumToDictionary(Items);
                                return;
                            }
                            continue;
                        }
                        if (reader.FieldCount != 5 || null == reader.GetValue(0) || null == reader.GetValue(1)) { Console.WriteLine(reader.FieldCount);continue; }
                        MenuItem item = new MenuItem();
                        item.Name = reader.GetValue(0).ToString();
                        item.Price = System.Convert.ToDecimal(reader.GetValue(1).ToString());
                        item.URL = reader.GetValue(2).ToString();
                        item.available = bool.Parse(reader.GetValue(3).ToString());
                        DateTime menuDate = DateTime.Parse(reader.GetValue(4).ToString());
                        if ((DateTime.Now - menuDate).TotalDays > 90){
                            Console.WriteLine("Menu more than 90 days old");
                            continue;
                        }
                        item.MenuDate = menuDate;

                        await _context.Items.AddAsync(item);
                        Console.WriteLine(item);
                    }
                }
                await _context.SaveChangesAsync(); 
                Items = await _context.Items
                .ToListAsync();
                enumToDictionary(Items);
            }


        }

        void printMenu() {
            foreach (KeyValuePair<int, List<MenuItem>> kvp in menu)
            {
                List<MenuItem> l = kvp.Value;
                String v = "";
                foreach (MenuItem item in l) { v += item; };
                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, v);
            }
        }

        //filer based on menu item name
        public async void OnGetFilter(string filterString) {
            errorMsg = "";
            if (filterString == null || filterString.Length == 0) filterString = "";
            filterStr = filterString;

            Items = await _context.Items
                .Where(item => item.Name.ToUpper().Contains(filterString.ToUpper()))
                .ToListAsync();
            enumToDictionary(Items);
        }

        // toggle available status for menu item
        public async void OnGetToggle(int id)
        {
            errorMsg = "";
            MenuItem item = _context.Items.Where(item => item.Id == id).FirstOrDefault();
            item.available = !item.available;
            _context.Items.Update(item);
            await _context.SaveChangesAsync();
            Items = await _context.Items
                .ToListAsync();
            enumToDictionary(Items);
        }

    }
}