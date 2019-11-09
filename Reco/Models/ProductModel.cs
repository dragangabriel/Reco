using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Reco.Models
{
    public class IndexModel
    {
        public IndexModel()
        {
            UltimeleProduse = new List<Product>();
            ToateProdusele = new List<Product>();
            UltimeleVandute = new List<Product>();
            Recomandari = new List<Product>();
            Take = 6;
        }
        public List<Product> UltimeleProduse { get; set; }
        public List<Product> ToateProdusele { get; set; }
        public List<Product> UltimeleVandute { get; set; }
        public List<Product> Recomandari { get; set; }
        public List<Category> ListaCategorii { get; set; }
        public int CategoryId { get; set; }
        public int TotalResults { get; set; }
        public int Take { get; set; }
        public int Skip { get; set; }
        public int TotalPages { get; set; }
        public int Page { get; set; }
        public string Search { get; set; }
        public string Order { get; set; }
    }

    public class ProductModel
    {
        public ProductModel()
        {
            ListaCategorii = new List<Category>();
        }

        public long Id { get; set; }

        [Display(Name = "Produs dezactivat?")]
        public bool Arata { get; set; }

        [Required(ErrorMessage = "Denumirea produsului este camp obligatoriu!")]
        [Display(Name = "Denumirea produsului")]
        public string Nume { get; set; }

        public List<Category> ListaCategorii { get; set; }

        [Required(ErrorMessage = "Categoria produsului este camp obligatoriu!")]
        [Display(Name = "Categoria produsului")]
        public int CategorieId { get; set; }

        [Required(ErrorMessage = "Pretul produsului este camp obligatoriu!")]
        [Display(Name = "Pretul produsului")]
        public decimal Pret { get; set; }
        
        [Display(Name = "Pret cu discount")]
        public decimal PretCuDiscount { get; set; }

        [Required(ErrorMessage = "Cantitatea produsului este camp obligatoriu!")]
        [Display(Name = "Cantitatea produsului")]
        public int Cantitate { get; set; }

        [Display(Name = "Descrierea produsului")]
        public string Descriere { get; set; }
        
        [Display(Name = "Incarcati o imagine")]
        public HttpPostedFileBase Image { get; set; }
    }

}