using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Divergent.Customers.Data.Models
{
    public class Order
    {
        public Guid CustomerId { get; set; }
        public Guid OrderId { get; set; }
    }
}
