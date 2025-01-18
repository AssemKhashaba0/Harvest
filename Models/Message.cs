using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string MessageContent { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
