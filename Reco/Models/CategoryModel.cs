using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Reco.Models
{
    public class CategoryModel
    {
        public long Id { get; set; }

        [Display(Name = "Categorie dezactivata?")]
        public bool Arata { get; set; }

        [Required(ErrorMessage = "Denumirea categoriei este camp obligatoriu!")]
        [Display(Name = "Denumirea categoriei")]
        public string Nume { get; set; }
        
        [Display(Name = "Incarcati o imagine")]
        public HttpPostedFileBase Image { get; set; }

        public string ImageUrl { get; set; }
    }

}