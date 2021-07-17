using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineJwelaryShop.Models
{
    public class SubCategoryWithCategory
    {
        public SubCategoryWithCategory()
        {

        }
        public SubCategoryWithCategory(sub_category s, category c)
        {
            //TO DO: Complete Member Initialization  
            this.Sub_Category = s;
            this.Category = c;
        }
        public sub_category Sub_Category { get; set; }
        public category Category{ get; set; }
    }
}