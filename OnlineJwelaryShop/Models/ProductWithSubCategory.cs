using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineJwelaryShop.Models
{
    public class ProductWithSubCategory
    {
        public ProductWithSubCategory()
        {

        }
        public ProductWithSubCategory(sub_category s, product p)
        {
            //TO DO: Complete Member Initialization  
            this.Sub_Category = s;
            this.Product = p;
        }
        public sub_category Sub_Category { get; set; }
        public product Product { get; set; }
    }
}