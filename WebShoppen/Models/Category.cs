using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShoppen.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // navigation properties
        public List<Product> Products { get; set; }
    }
}
