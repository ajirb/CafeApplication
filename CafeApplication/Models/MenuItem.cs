using System.ComponentModel.DataAnnotations;

namespace CafeApplication.Models
{
    public class MenuItem
    {
        public uint Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [Required]
        public decimal Price { get; set; }
        public string? URL { get; set; }
        public bool available { get; set; } = true;
        public DateTime MenuDate { get; set; } = DateTime.Now.Date;

        public override string ToString()
        {
            return "[ Id : " + Id + " Name : " + Name + " Price : " + Price + " URL : " + URL +
                " available : " + available + " MenuDate : " + MenuDate+" ] ";
        }

        public String getAvailabeStatus() {
            if (available) return "checked";
            return "";
        }
    }
}
