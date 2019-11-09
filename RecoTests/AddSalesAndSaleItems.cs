using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reco.Models;
using System.Collections.Generic;
using System.Linq;

namespace RecoTests
{
    [TestClass]
    public class AddSalesAndSaleItems
    {
        [TestMethod]
        public void TestMethod1()
        {
            return;
            Random gen = new Random();

            for (int i = 1; i <= 200; i++)
            {
                DateTime start = new DateTime(2010, 1, 1);
                int range = (start.AddYears(5) - start).Days;
                start = start.AddDays(gen.Next(range));
                var productIds = new List<int>();

                using (RecoEntities recoEntities = new RecoEntities())
                {
                    var productsNumber = gen.Next(5) + 1;
                    var userId = gen.Next(19) + 2005;
                    productIds.Clear();
                    for (int j = 1; j <= productsNumber; j++)
                        productIds.Add(gen.Next(49) + 4009);

                    decimal sum = 0;
                    foreach (var productId in productIds)
                    {
                        var product = recoEntities.Products.Find(productId);
                        sum += product.PretCuDiscount;
                    }

                    var newSale = new Sale()
                    {
                        Address = "-",
                        Phone = "0790649098",
                        CreatedDate = start,
                        Price = sum,
                        UserId = userId
                    };
                    recoEntities.Sales.Add(newSale);
                    recoEntities.SaveChanges();

                    while (productIds.Count > 0)
                    {
                        var newItemSale = new SaleItem()
                        {
                            CreatedDate = start,
                            SaleId = newSale.Id,
                            UserId = newSale.UserId,
                            ProductId = productIds[0],
                            Quantity = productIds.Count(x => x == productIds[0])
                        };
                        recoEntities.SaleItems.Add(newItemSale);
                        recoEntities.SaveChanges();

                        var id = productIds[0];
                        productIds.RemoveAll(x => x == id);
                    }            

                }
            }
        }
    }
}
