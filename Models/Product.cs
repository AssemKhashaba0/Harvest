using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم المنتج مطلوب")]
        [StringLength(100, ErrorMessage = "اسم المنتج يجب ألا يتجاوز 100 حرف")]
        public string Name { get; set; }

        [Required(ErrorMessage = "وصف المنتج مطلوب")]
        [StringLength(500, ErrorMessage = "وصف المنتج يجب ألا يتجاوز 500 حرف")]
        public string Description { get; set; }
        [ValidateNever]
        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "بلد المنشأ مطلوب")]
        [StringLength(100, ErrorMessage = "اسم بلد المنشأ يجب ألا يتجاوز 100 حرف")]
        public string OriginCountry { get; set; }

        [Required(ErrorMessage = "تفاصيل التغليف مطلوبة")]
        [StringLength(500, ErrorMessage = "تفاصيل التغليف يجب ألا تتجاوز 500 حرف")]
        public string PackagingDetails { get; set; }

        [Required(ErrorMessage = "السعر مطلوب")]
        [Range(0.01, double.MaxValue, ErrorMessage = "السعر يجب أن يكون أكبر من صفر")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "القسم مطلوب")]
        public int CategoryId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public Category Category { get; set; }  // Navigation property
    }
}
