using AutoMapper;
using PagedList;
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
    public class CategoriesController : Controller
    {
        RecoEntities recoEntities = new RecoEntities();

        public CategoriesController()
        { }

        public CategoriesController(RecoEntities recoEntities)
        {
            this.recoEntities = recoEntities;
        }

        public ActionResult Index(int page = 1, int pageSize = 10)
        {
            if (Session["role"] == null || Session["role"].ToString() != "Admin")
                return View("~/Shared/Error");

            List<CategoryModel> allCategories = new List<CategoryModel>();
            allCategories = recoEntities.Categories.Select(x => new CategoryModel() { Id = x.Id, Nume = x.Nume, ImageUrl = x.ImageUrl }).ToList();

            return View(allCategories);
        }

        public ActionResult AddCategory()
        {
            if (Session["role"] == null || Session["role"].ToString() != "Admin")
                return View("~/Shared/Error");

            var model = new CategoryModel();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCategory(CategoryModel model, HttpPostedFileBase image)
        {
            try
            {
                if (Session["role"] == null || Session["role"].ToString() != "Admin")
                    return View("~/Shared/Error");

                if (recoEntities.Categories.Any(x => x.Nume == model.Nume))
                {
                    ModelState.AddModelError("", "Deja exista un produs cu acest nume");
                }

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                Mapper.Initialize(cfg => cfg.CreateMap<CategoryModel, Category>());
                var category = Mapper.Map<CategoryModel, Category>(model);
                category.DataCreare = DateTime.Now;

                recoEntities.Categories.Add(category);
                recoEntities.SaveChanges();

                if (image != null)
                {
                    System.IO.File.Delete(Path.Combine(Server.MapPath("/images/"), "category" + category.Id.ToString() + ".jpg"));
                    string path = Path.Combine(Server.MapPath("/images/"), "category" + category.Id.ToString() + ".jpg");
                    image.SaveAs(path);
                    category.ImageUrl = "category" + category.Id.ToString() + ".jpg";
                }
                recoEntities.SaveChanges();

                return RedirectToAction("Index", "Categories", new { area = "" });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Categories", new { area = "" });
            }
        }

        public ActionResult EditCategory(int categoryId)
        {
            if (Session["role"] == null || Session["role"].ToString() != "Admin")
                return View("~/Shared/Error");

            if (!recoEntities.Categories.Any(x => x.Id == categoryId))
            {
                return View("~/Shared/Error");
            }

            var category = recoEntities.Categories.Single(x => x.Id == categoryId);

            Mapper.Initialize(cfg => cfg.CreateMap<Category, CategoryModel>());
            var model = Mapper.Map<Category, CategoryModel>(category);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCategory(CategoryModel model, HttpPostedFileBase image)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var category = recoEntities.Categories.Single(x => x.Id == model.Id);

                if (recoEntities.Categories.Any(x => x.Nume == model.Nume) && model.Nume != category.Nume)
                {
                    ModelState.AddModelError("", "Deja exista un produs cu acest nume");
                    return View(model);
                }

                category.Nume = model.Nume;
                recoEntities.SaveChanges();

                if (image != null)
                {
                    System.IO.File.Delete(Path.Combine(Server.MapPath("/images/"), "category" + category.Id.ToString() + ".jpg"));
                    string path = Path.Combine(Server.MapPath("/images/"), "category" + category.Id.ToString() + ".jpg");
                    image.SaveAs(path);
                    category.ImageUrl = "category" + category.Id.ToString() + ".jpg";
                }
                recoEntities.SaveChanges();

                return RedirectToAction("Index", "Categories", new { area = "" });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Categories", new { area = "" });
            }
        }
    }
}