using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reco.Models
{
    public class SimModel
    {
        public SimModel()
        {
            Similarities = new List<SimilarityModel>();
            ProductsUser2 = new List<string>();
            ProductsUser1 = new List<string>();
        }

        public int UserId { get; set; }
        public List<SimilarityModel> Similarities { get; set; }
        public List<string> ProductsUser1 { get; set; }
        public List<string> ProductsUser2 { get; set; }
    }

    public class SimilarityModel
    {
        public int UserId { get; set; }
        public double Similarity { get; set; }
        public string UserName { get; set; }
    }

}