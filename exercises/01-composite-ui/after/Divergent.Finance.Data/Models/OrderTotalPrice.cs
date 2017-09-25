using System;

namespace Divergent.Finance.Data.Models
{
    public class OrderTotalPrice
    {
        public Guid OrderId { get; set; }
        public double TotalPrice { get; set; }
    }
}
