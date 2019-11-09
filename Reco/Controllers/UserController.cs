using Reco.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Reco.Controllers
{
    public class UserController : Controller
    {
        RecoEntities recoEntities = new RecoEntities();

        public UserController()
        { }

        public UserController(RecoEntities recoEntities)
        {
            this.recoEntities = recoEntities;
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginUserModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!recoEntities.Users.ToList().Any(x => x.Email == model.Email && x.Password == model.Password))
            {
                ModelState.AddModelError("", "User inexistent sau parola incorecta");
                return View(model);
            }

            var user = recoEntities.Users.Single(x => x.Email == model.Email && x.Password == model.Password);

            Session["userId"] = user.Id;
            Session["user"] = user.Forename + " " + user.Surname;
            Session["role"] = user.Role;
            if (user.ImageUrl != null)
                Session["imageUrl"] = user.ImageUrl;
            Session["shoppingCart"] = new ShoppingCartModel();

            return RedirectToAction("Index", "Home", new { area = "" });
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterUserModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("", model);
            }

            if (recoEntities.Users.ToList().Any(x => x.Email == model.Email))
            {
                ModelState.AddModelError("", "Adresa de email deja introdusa in sistem!");
                return View(model);
            }

            var newUser = new User()
            {
                Forename = model.Forename,
                Surname = model.Surname,
                Email = model.Email,
                Password = model.Password,
                Role = "Vizitator",
                DataCreare = DateTime.Now
            };

            recoEntities.Users.Add(newUser);
            recoEntities.SaveChanges();

            var user = newUser;

            Session["userId"] = user.Id;
            Session["user"] = user.Forename + " " + user.Surname;
            Session["role"] = user.Role;
            if (user.ImageUrl != null)
                Session["imageUrl"] = user.ImageUrl;
            Session["shoppingCart"] = new ShoppingCartModel();

            return RedirectToAction("Index", "Home", new { area = "" });
        }

        public ActionResult Logout()
        {
            Session["userId"] = null;
            Session["user"] = null;
            Session["role"] = null;
            Session["imageUrl"] = null;
            Session["shoppingCart"] = null;

            return RedirectToAction("Index", "Home", new { area = "" });
        }

        public ActionResult UserProfile(long userId)
        {
            if (!recoEntities.Users.Any(x => x.Id == userId))
            {
                return View("~/Shared/Error");
            }

            var user = recoEntities.Users.Single(x => x.Id == userId);

            var model = new EditProfileModel()
            {
                Id = user.Id,
                Surname = user.Surname,
                Forename = user.Forename,
                Email = user.Email,
                ImageUrl = user.ImageUrl
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult UserProfile(EditProfileModel model, HttpPostedFileBase avatar)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = recoEntities.Users.Single(x => x.Id == model.Id);
            user.Forename = model.Forename;
            user.Surname = model.Surname;

            if (avatar != null)
            {                
                System.IO.File.Delete(Path.Combine(Server.MapPath("/images/"), user.Id.ToString() + ".jpg"));
                string fileName = Guid.NewGuid() + Path.GetFileName(avatar.FileName);
                string path = Path.Combine(Server.MapPath("/images/"), user.Id.ToString() + ".jpg");
                avatar.SaveAs(path);
                user.ImageUrl = model.Id.ToString() + ".jpg";
            }

            recoEntities.SaveChanges();

            Session["userId"] = user.Id;
            Session["user"] = user.Forename + " " + user.Surname;
            Session["role"] = user.Role;
            if (user.ImageUrl != null)
                Session["imageUrl"] = user.ImageUrl;

            return RedirectToAction("UserProfile", new { userId = model.Id });
        }

    }
}