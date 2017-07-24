using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Divergent.Finance.Data.Models
{
    public class OrderTotalPrice
    {
        public Guid OrderId { get; set; }
        public double TotalPrice { get; set; }
    }
}
