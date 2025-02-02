using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShoppen.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        // navigation properties
        public User User { get; set; }
        public List<CartItem> Items { get; set; }
    }
}
