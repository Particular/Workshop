using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Divergent.Customers.Data.Migrations;
using Raven.Client;
using Raven.Client.Document;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Divergent.Customers.API
{
    public static class RavenConfig
    {
        public static void Config(IWindsorContainer container)
        {
            var store = new DocumentStore()
            {
                DefaultDatabase = "Divergent.Customers",
                ConnectionStringName = "Divergent.Customers"
            }.Initialize();

            container.Register(Component.For<IDocumentStore>().Instance(store));

            using (var session = store.OpenSession())
            {
                dynamic seed = session.Load<dynamic>("Data/Seed");
                if (seed == null)
                {
                    session.Store(new { }, "Data/Seed");

                    SeedData.Customers().ForEach(c =>
                    {
                        session.Store(c);
                    });

                    session.SaveChanges();
                }
            }

        }
    }
}