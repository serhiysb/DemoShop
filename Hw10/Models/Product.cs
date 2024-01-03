using System.ComponentModel.DataAnnotations;

namespace Hw10.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Введіть назву продукту")]
        [Display(Name = "Product Name")]
        public string? Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public string? Color { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        public string? Image { get; set; }
        [Required]
        public int CategoryId { get; set; }

        public Category Category { get; set; }
    }
}
