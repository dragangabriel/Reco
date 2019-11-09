using Reco.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Reco.Controllers
{
    public class ShoppingCartController : Controller
    {
        RecoEntities recoEntities = new RecoEntities();

        public ShoppingCartController()
        { }

        public ShoppingCartController(RecoEntities recoEntities)
        {
            this.recoEntities = recoEntities;
        }

        public ActionResult AddProduct(int productId)
        {
            var shoppingCart = Session["shoppingCart"] as ShoppingCartModel;

            var product = recoEntities.Products.Single(x => x.Id == productId);

            shoppingCart.TotalItems ++;
            shoppingCart.TotalPrice += product.PretCuDiscount;

            if (shoppingCart.Items.Any(x => x.ProductId == productId))
            {
                foreach (var item in shoppingCart.Items.Where(x => x.ProductId == productId))
                    item.Cantity++;
            } else
            {
                shoppingCart.Items.Add(new ItemModel()
                {
                    ProductId = productId,
                    ProductName = product.Nume,
                    ProductPrice = product.PretCuDiscount,
                    Cantity = 1,
                    CategoryId = product.CategorieId,
                    CategoryName = product.Category.Nume,
                    ImageUrl = product.ImageUrl
                });
            }

            Session["shoppingCart"] = shoppingCart;
            Session["showNotification"] = product.Nume;
            return RedirectToAction("ViewProduct", "Home", new { productId = productId });
        }

        public ActionResult ViewShoppingCart()
        {
            if (Session["shoppingCart"] == null)
            {
                return View("~/Shared/Error");
            }

            var model = Session["shoppingCart"] as ShoppingCartModel;

            if (Session["showNotification"] != null)
            {
                ViewBag.Error = Session["showNotification"] .ToString() + " a fost sters din cos!";
                Session["showNotification"] = null;
            }
            return View(model);
        }

        public ActionResult RemoveProduct(int productId)
        {
            if (Session["shoppingCart"] == null)
            {
                return View("~/Shared/Error");
            }

            var shoppingCart = Session["shoppingCart"] as ShoppingCartModel;

            if (shoppingCart.Items.Any(x => x.ProductId == productId))
            {
                foreach (var item in shoppingCart.Items.Where(x => x.ProductId == productId))
                {
                    item.Cantity--;
                    shoppingCart.TotalItems--;
                    shoppingCart.TotalPrice -= item.ProductPrice;
                }

                shoppingCart.Items.RemoveAll(x => x.Cantity == 0);
            }
            var product = recoEntities.Products.Single(x => x.Id == productId);

            Session["shoppingCart"] = shoppingCart;
            Session["showNotification"] = product.Nume;
            return RedirectToAction("ViewShoppingCart");
        }

        public ActionResult Checkout()
        {
            if (Session["shoppingCart"] == null)
            {
                return View("~/Shared/Error");
            }

            var model = Session["shoppingCart"] as ShoppingCartModel;

            if (model.TotalPrice <= 0)
            {
                return View("~/Shared/Error");
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Success(string numarTelefon, string adresa)
        {
            if (numarTelefon == null || adresa == null || numarTelefon == "" || adresa == "")
            {
                ViewBag.Error = "Numarul de telefon si adresa sunt campuri obligatorii!";
                var model = Session["shoppingCart"] as ShoppingCartModel;
                return View("Checkout", model);
            }

            if (numarTelefon.Length < 10)
            {
                ViewBag.Error = "Numarul de telefon nu este complet!";
                var model = Session["shoppingCart"] as ShoppingCartModel;
                return View("Checkout", model);
            }

            if (Session["shoppingCart"] == null)
            {
                return View("~/Shared/Error");
            }

            var shoppingCart = Session["shoppingCart"] as ShoppingCartModel;

            var sale = new Sale()
            {
                Price = shoppingCart.TotalPrice,
                UserId = Int32.Parse(Session["userId"].ToString()),
                Address = adresa,
                Phone = numarTelefon,
                CreatedDate = DateTime.Now                
            };

            recoEntities.Sales.Add(sale);
            recoEntities.SaveChanges();

            foreach (var item in shoppingCart.Items)
            {
                var saleItem = new SaleItem()
                {
                    ProductId = item.ProductId,
                    CreatedDate = DateTime.Now,
                    SaleId = sale.Id,
                    UserId = Int32.Parse(Session["userId"].ToString()),
                    Quantity = item.Cantity
                };
                recoEntities.SaleItems.Add(saleItem);
                recoEntities.SaveChanges();

                var product = recoEntities.Products.Single(x => x.Id == item.ProductId);
                product.Cantitate -= item.Cantity;
                recoEntities.SaveChanges();
            }

            Session["shoppingCart"] = new ShoppingCartModel(); ;
            ViewBag.TransactionId = sale.Id;

            return View();
        }
    }
}