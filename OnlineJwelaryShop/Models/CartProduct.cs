using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace OnlineJwelaryShop.Models
{
    public class CartProduct
    {
        public DataTable ProductTable { get; set; }
        public int Quantity { get; set; }
    }
}