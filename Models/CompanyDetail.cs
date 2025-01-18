using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class CompanyDetail
    {
        public int Id { get; set; }
        public string LogoUrl { get; set; }
        public string AboutText { get; set; }
        public string Vision { get; set; }
        public string Mission { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
