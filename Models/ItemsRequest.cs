using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace ATKInventory.API.Models
{
    public class ItemsRequest
    {
        [Required]
        [StringLength(20)]
        public string ItemCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string ItemName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; } = decimal.Zero;

        [Required]
        public int Stock {  get; set; } = int.MinValue;
    }
}
