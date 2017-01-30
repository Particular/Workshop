using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Divergent.Finance.Messages.Commands
{
    public class InitiatePaymentProcessCommand
    {
        public double Amount { get; set; }
        public int CustomerId { get; set; }
        public int OrderId { get; set; }
    }
}
