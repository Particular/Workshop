using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Divergent.Finance.Messages.Commands
{
    class InitiatePaymentProcessCommand
    {
        public double Amount { get; internal set; }
        public int CustomerId { get; internal set; }
        public int OrderId { get; internal set; }
    }
}
