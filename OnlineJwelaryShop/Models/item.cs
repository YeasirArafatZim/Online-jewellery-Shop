using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace OnlineJwelaryShop.Models
{
    public class item
    {
        public product Product { get; set; }
        public int Quantity { get; set; }

        public DataTable ProductCartTable { get; set; }

        public int carttracnumber { get; set; }

        public int totalcart { get; set; }
    }
}