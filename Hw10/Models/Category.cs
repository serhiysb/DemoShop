using System.ComponentModel.DataAnnotations;

namespace Hw10.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Назва категорії")]
        public string? Name { get; set; }
        public List<Product>? Products { get; set; }
    }
}