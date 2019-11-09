using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reco.Models
{
    public class IndexSalesModel
    {
        public IndexSalesModel()
        {
            Sales = new List<Sale>();
            SaleItems = new List<SaleItem>();
        }

        public List<Sale> Sales { get; set; }
        public List<SaleItem> SaleItems { get; set; }
    }
}