using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reco.Models;

namespace RecoTests
{
    [TestClass]
    public class AddUsers
    {
        [TestMethod]
        public void TestMethod1()
        {
            return;
            Random gen = new Random();

            for (int i = 2; i <= 20; i++)
            {
                DateTime start = new DateTime(2000, 1, 1);
                int range = (start.AddYears(5) - start).Days;
                start = start.AddDays(gen.Next(range));
                using (RecoEntities recoEntities = new RecoEntities())
                {
                    recoEntities.Database.Connection.Open();
                    var user = new User()
                    {
                        DataCreare = start,
                        Email = "user" + i.ToString() + "@mailinator.com",
                        Forename = "User",
                        Surname = i.ToString(),
                        ImageUrl = null,
                        Password = "12345678",
                        Role = "Vizitator"
                    };

                    recoEntities.Users.Add(user);
                    recoEntities.SaveChanges();
                }
            }
        }
    }
}
