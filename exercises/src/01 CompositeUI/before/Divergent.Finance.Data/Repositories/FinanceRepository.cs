using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Divergent.Finance.Data.Models;

namespace Divergent.Finance.Data.Repositories
{
    public class FinanceRepository : IFinanceRepository
    {
        public async Task<List<Price>> Prices()
        {
            return SeedPrices();
        }

        public async Task<Price> Price(Guid productId)
        {
            return SeedPrices().First(s => s.ProductId == productId);
        }

        private List<Price> SeedPrices()
        {
            var thePhantomMenace = Guid.Parse("77158b05-437d-4aa7-baaa-05df0bc60f17");
            var attackOfTheClones = Guid.Parse("a5f4fc6d-9eb7-41b7-ac93-e294cf2cc2fa");
            var revengeOfTheSith = Guid.Parse("e534c697-9bf2-4f7f-bc2c-73ec8f2c9f8b");
            var aNewHope = Guid.Parse("524d2015-1240-4f0c-85af-17826f8d1e45");
            var theEmpireStrikesBack = Guid.Parse("099952ec-2ab5-4334-a0d5-20fdb63daadb");
            var returnOfTheJedi = Guid.Parse("11cb4ecd-5383-4da2-b3d4-8fd2a4e2117f");
            var theForceAwakens = Guid.Parse("5e449efa-3f48-45d2-82f0-22483d97516a");

            return new List<Price>()
            {
                new Price() { ProductId = thePhantomMenace, ItemPrice = 10 },
                new Price() { ProductId = attackOfTheClones, ItemPrice = 10 },
                new Price() { ProductId = revengeOfTheSith, ItemPrice = 10 },
                new Price() { ProductId = aNewHope, ItemPrice = 15 },
                new Price() { ProductId = theEmpireStrikesBack, ItemPrice = 15 },
                new Price() { ProductId = returnOfTheJedi, ItemPrice = 15 },
                new Price() { ProductId = theForceAwakens, ItemPrice = 25 }
            };
        }
    }
}
