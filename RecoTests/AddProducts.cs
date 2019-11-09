using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reco.Models;

namespace RecoTests
{
    [TestClass]
    public class AddProducts
    {
        [TestMethod]
        public void TestMethod1()
        {
            //return;
            Random gen = new Random();

            for ( int i = 1; i <= 50; i++)
            {
                DateTime start = new DateTime(2005, 1, 1);
                int range = (start.AddYears(5) - start).Days;
                start = start.AddDays(gen.Next(range));
                var categoryId = i % 4 + 1;
                decimal price = gen.Next(range) + 1;
                using (RecoEntities recoEntities = new RecoEntities())
                {
                    recoEntities.Database.Connection.Open();
                    var product = new Product()
                    {
                        DataCreare = start,
                        CategorieId = categoryId,
                        Nume = (categoryId).ToString() + " " + ((i / 4) + 1),
                        Pret = price,
                        PretCuDiscount = price,
                        Cantitate = gen.Next(range)
                    };

                    recoEntities.Products.Add(product);
                    recoEntities.SaveChanges();
                }
            }
        }
    }
}
