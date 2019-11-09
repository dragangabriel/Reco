using Reco.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Reco.Controllers
{
    public class SimController : Controller
    {
        RecoEntities recoEntities = new RecoEntities();

        public SimController()
        { }

        public SimController(RecoEntities recoEntities)
        {
            this.recoEntities = recoEntities;
        }
        // GET: Sim
        public ActionResult Index(int userId)
        {
            var model = new SimModel();
            model.UserId = userId;

            var similarities = getAllUsersSimilarity(userId);
            var descendingSimilarities = similarities.OrderByDescending(x => x.Similarity);

            model.Similarities = descendingSimilarities.ToList();

            foreach (var sim in model.Similarities)
            {
                var user = recoEntities.Users.Single(x => x.Id == sim.UserId);
                sim.UserName = user.Forename + " " + user.Surname;
            }

            return View(model);
        }

        public ActionResult Index2(int user1, int user2)
        {
            var model = new SimModel();

            var products = recoEntities.Products.ToList();
            var user1P = recoEntities.UserProducts.Where(x => x.UserId == user1).Select(x => x.ProductId).ToList();
            var user2P = recoEntities.UserProducts.Where(x => x.UserId == user2).Select(x => x.ProductId).ToList();
            
            foreach(var prod in products)
            {
                if (user1P.Contains(prod.Id))
                    model.ProductsUser1.Add(prod.Nume);
                else model.ProductsUser1.Add("-");
                if (user2P.Contains(prod.Id))
                    model.ProductsUser2.Add(prod.Nume);
                else model.ProductsUser2.Add("-");
            }

            return View(model);
        }


        private List<SimilarityModel> getAllUsersSimilarity(int userId)
        {
            List<SimilarityModel> similaritites = new List<SimilarityModel>();
            var userIds = recoEntities.Users.Where(x => x.Id != userId).Select(x => x.Id).ToList();
            foreach (var id in userIds)
            {
                double similarity = getSimilarityBetweenTwoUsers(userId, id);
                similaritites.Add(new SimilarityModel() { UserId = id, Similarity = similarity });
            }
            return similaritites;
        }

        private double dotProduct(List<int> a, List<int> b)
        {
            double res = 0;
            for (var i = 0; i < a.Count; i++)
            {
                res = res + (a[i] * b[i]);
            }
            return res;
        }

        private double magnitude(List<int> a)
        {
            return Math.Sqrt(dotProduct(a, a));
        }

        private double getSimilarityBetweenTwoUsers(int user1, int user2)
        {
            double res = 0;
            var user1Prod = recoEntities.UserProducts.Where(x => x.UserId == user1).Select(x => x.ProductId).ToList();
            var user2Prod = recoEntities.UserProducts.Where(x => x.UserId == user2).Select(x => x.ProductId).ToList();
            var user1Vector = new List<int>();
            var user2Vector = new List<int>();

            var products = recoEntities.Products.Select(x => x.Id).ToList();
            foreach (var productId in products)
            {
                if (user1Prod.Contains(productId))
                    user1Vector.Add(1);
                else
                    user1Vector.Add(0);
                if (user2Prod.Contains(productId))
                    user2Vector.Add(1);
                else
                    user2Vector.Add(0);
            }

            var dotProd = dotProduct(user1Vector, user2Vector);
            var magProd = magnitude(user1Vector) * magnitude(user2Vector);

            if (magProd == 0)
                return 0;

            res = dotProd / magProd;

            return res;
        }

    }
}