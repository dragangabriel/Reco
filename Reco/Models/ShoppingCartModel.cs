using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reco.Models
{
    public class ShoppingCartModel
    {
        public ShoppingCartModel()
        {
            TotalPrice = 0;
            TotalItems = 0;
            Items = new List<ItemModel>();
        }

        public decimal TotalPrice { get; set; }
        public int TotalItems { get; set; }
        public List<ItemModel> Items { get; set; }
    }

    public class ItemModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public int Cantity { get; set; }
        public decimal ProductPrice { get; set; }
        public decimal CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}