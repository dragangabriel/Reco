using AutoMapper;
using Reco.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Reco.Controllers
{
    public class HomeController : Controller
    {
        RecoEntities recoEntities = new RecoEntities();

        public HomeController()
        { }

        public HomeController(RecoEntities recoEntities)
        {
            this.recoEntities = recoEntities;
        }

        public ActionResult Index(int page = 1, int categoryId = 0, string search = null, string sort = null)
        {
            var model = new IndexModel();
            model.UltimeleProduse = recoEntities.Products.Where(x => x.Arata == false).OrderByDescending(x => x.DataCreare).Take(4).ToList();
            model.UltimeleVandute = recoEntities.SaleItems.OrderByDescending(x => x.CreatedDate).Select(x => x.Product).Distinct().Where(x => x.Arata == false).Take(4).ToList();
            if (Session["userId"] != null)
                model.Recomandari = getRecommendations(Convert.ToInt32(Session["userId"].ToString()));
            model.ToateProdusele = recoEntities.Products.ToList();
            if (Session["role"] == null || Session["role"].ToString() != "Admin")
                model.ToateProdusele = model.ToateProdusele.Where(x => x.Arata == false).ToList();
            if (categoryId != 0)
                model.ToateProdusele = model.ToateProdusele.Where(x => x.CategorieId == categoryId).ToList();
            if (search != null)
                model.ToateProdusele = model.ToateProdusele.Where(x => x.Nume.ToLower().Contains(search.ToLower()) || x.Descriere.ToLower().Contains(search.ToLower())).ToList();
            if (sort != null)
            {
                if (sort == "asc")
                {
                    model.ToateProdusele = model.ToateProdusele.OrderBy(x => x.PretCuDiscount).ToList();
                    model.Order = "Ordonate crescator dupa pret";
                }
                if (sort == "desc")
                {
                    model.ToateProdusele = model.ToateProdusele.OrderByDescending(x => x.PretCuDiscount).ToList();
                    model.Order = "Ordonate descrescator dupa pret";
                }
                if (sort == "ascX")
                {
                    model.ToateProdusele = model.ToateProdusele.OrderBy(x => x.Nume.ToLower()).ToList();
                    model.Order = "Ordonate crescator dupa nume";
                }
                if (sort == "descX")
                {
                    model.ToateProdusele = model.ToateProdusele.OrderByDescending(x => x.Nume.ToLower()).ToList();
                    model.Order = "Ordonate descrescator dupa nume";
                }
            }
            else
            {
                model.Order = "Ordoneaza produsele";
            }
            int productsNumber = model.ToateProdusele.Count;

            model.TotalResults = productsNumber;
            model.Page = page;
            model.Skip = (page - 1) * 9;
            model.Take = 9;
            model.TotalPages = productsNumber / 9 + (productsNumber % 9 > 0 ? 1 : 0);
            model.ToateProdusele = model.ToateProdusele.Skip(model.Skip).Take(model.Take).ToList();
            model.Search = search;
            model.ListaCategorii = recoEntities.Categories.ToList();
            model.CategoryId = categoryId;

            return View(model);
        }

        private List<Product> getRecommendations(int userId)
        {
            var similarities = getAllUsersSimilarity(userId);
            var descendingSimilarities = similarities.OrderByDescending(x => x.Value);
            var res = new List<int>();
            var ourUserProducts = recoEntities.UserProducts.Where(x => x.UserId == userId).Select(x => x.ProductId).ToList();
            foreach (var sim in descendingSimilarities)
            {
                var thisUserProd = recoEntities.UserProducts.Where(x => x.UserId == sim.Key).Select(x => x.ProductId).ToList();
                foreach(var prod in thisUserProd)
                {
                    if (!ourUserProducts.Contains(prod) && !res.Contains(prod))
                    {
                        res.Add(prod);
                        if (res.Count == 8)
                        {
                            var response = new List<Product>();
                            foreach(var r in res)
                            {
                                response.Add(recoEntities.Products.Single(x => x.Id == r));
                            }
                            return response;
                        }
                    }
                }
            }
            return new List<Product>();
        }

        private Dictionary<int, double> getAllUsersSimilarity(int userId)
        {
            Dictionary<int, double> similaritites = new Dictionary<int, double>();
            var userIds = recoEntities.Users.Where(x => x.Id != userId).Select(x => x.Id).ToList();
            foreach (var id in userIds)
            {
                double similarity = getSimilarityBetweenTwoUsers(userId, id);
                similaritites.Add(id, similarity);
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
            foreach(var productId in products)
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

        

        public ActionResult AddProduct()
        {
            if (Session["role"] == null || Session["role"].ToString() != "Admin")
                return View("~/Shared/Error");

            var model = new ProductModel();
            model.ListaCategorii = recoEntities.Categories.Where(x => x.Arata == false).ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddProduct(ProductModel model, HttpPostedFileBase image)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    model.ListaCategorii = recoEntities.Categories.Where(x => x.Arata == false).ToList();
                    return View(model);
                }

                if (recoEntities.Products.Any(x => x.Nume == model.Nume))
                {
                    ModelState.AddModelError("", "Deja exista un produs cu acest nume");
                }

                Mapper.Initialize(cfg => cfg.CreateMap<ProductModel, Product>());
                var product = Mapper.Map<ProductModel, Product>(model);
                product.DataCreare = DateTime.Now;

                if (product.PretCuDiscount == 0)
                {
                    product.PretCuDiscount = product.Pret;
                }

                recoEntities.Products.Add(product);
                recoEntities.SaveChanges();

                if (image != null)
                {
                    System.IO.File.Delete(Path.Combine(Server.MapPath("/images/"), "prod" + product.Id.ToString() + ".jpg"));
                    string path = Path.Combine(Server.MapPath("/images/"), "prod" + product.Id.ToString() + ".jpg");
                    image.SaveAs(path);
                    product.ImageUrl = "prod" + product.Id.ToString() + ".jpg";
                }
                recoEntities.SaveChanges();

                return RedirectToAction("Index", "Home", new { area = "" });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
        }

        public ActionResult EditProduct(int productId)
        {
            if (Session["role"] == null || Session["role"].ToString() != "Admin")
                return View("~/Shared/Error");

            if (!recoEntities.Products.Any(x => x.Id == productId))
            {
                return View("~/Shared/Error");
            }

            var product = recoEntities.Products.Single(x => x.Id == productId);

            Mapper.Initialize(cfg => cfg.CreateMap<Product, ProductModel>());
            var model = Mapper.Map<Product, ProductModel>(product);
            model.ListaCategorii = recoEntities.Categories.ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProduct(ProductModel model, HttpPostedFileBase image)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var product = recoEntities.Products.Single(x => x.Id == model.Id);

                Mapper.Initialize(cfg => cfg.CreateMap<ProductModel, Product>());
                Mapper.Map<ProductModel, Product>(model, product);

                if (product.PretCuDiscount == 0)
                {
                    product.PretCuDiscount = product.Pret;
                }

                recoEntities.SaveChanges();

                if (image != null)
                {
                    System.IO.File.Delete(Path.Combine(Server.MapPath("/images/"), "prod" + product.Id.ToString() + ".jpg"));
                    string path = Path.Combine(Server.MapPath("/images/"), "prod" + product.Id.ToString() + ".jpg");
                    image.SaveAs(path);
                    product.ImageUrl = "prod" + product.Id.ToString() + ".jpg";
                }
                recoEntities.SaveChanges();

                return RedirectToAction("Index", "Home", new { area = "" });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
        }

        public ActionResult ViewProduct(int productId)
        {
            if (!recoEntities.Products.Any(x => x.Id == productId))
            {
                return View("~/Shared/Error");
            }

            var product = recoEntities.Products.Single(x => x.Id == productId);

            if (Session["shoppingCart"] != null)
            {
                var shoppingCart = Session["shoppingCart"] as ShoppingCartModel;

                foreach (var item in shoppingCart.Items.Where(x => x.ProductId == productId))
                {
                    product.Cantitate -= item.Cantity;
                }
            }

            if (Session["showNotification"] != null)
            {
                ViewBag.Success = Session["showNotification"].ToString() + " a fost adaugat in cos!";
                Session["showNotification"] = null;
            }
            return View(product);
        }
    }
}