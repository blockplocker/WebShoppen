using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShoppen.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Total { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal VAT { get; set; }
        public int UserId { get; set; }
        public int CustomerId { get; set; }

        // navigation properties
        public User User { get; set; }
        public Customer Customer { get; set; }
        public List<OrderItem> Items { get; set; }
    }
}
