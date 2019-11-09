using Reco.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Reco.Controllers
{
    public class SalesController : Controller
    {
        RecoEntities recoEntities = new RecoEntities();

        public SalesController()
        { }

        public SalesController(RecoEntities recoEntities)
        {
            this.recoEntities = recoEntities;
        }

        [HttpGet]
        public ActionResult Index()
        {
            if (Session["role"] == null || Session["role"].ToString() != "Admin")
            {
                return View("~/Shared/Error");
            }

            var model = new IndexSalesModel();

            model.Sales = recoEntities.Sales.ToList();
            model.SaleItems = recoEntities.SaleItems.ToList();

            return View(model);
        }

        [HttpGet]
        public ActionResult IndexUser(int userId)
        {
            if (!recoEntities.Users.Any(x => x.Id == userId))
            {
                return View("~/Shared/Error");
            }

            var model = new IndexSalesModel();

            model.Sales = recoEntities.Sales.Where(x => x.UserId == userId).ToList();
            model.SaleItems = recoEntities.SaleItems.Where(x => x.UserId == userId).ToList();

            return View("Index", model);
        }
    }
}