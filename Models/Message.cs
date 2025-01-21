using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Message
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "الاسم مطلوب")]
        [StringLength(100, ErrorMessage = "الاسم يجب ألا يتجاوز 100 حرف")]
        public string Name { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [Phone(ErrorMessage = "الرجاء إدخال رقم هاتف صحيح")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "محتوى الرسالة مطلوب")]
        [StringLength(1000, ErrorMessage = "محتوى الرسالة يجب ألا يتجاوز 1000 حرف")]
        public string MessageContent { get; set; }

        [Required(ErrorMessage = "التاريخ مطلوب")]
        public DateTime CreatedAt { get; set; }

    }
}
