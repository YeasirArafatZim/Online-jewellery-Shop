//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OnlineJwelaryShop.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Order_details
    {
        public int order_id { get; set; }
        public int unique_id { get; set; }
        public string user_id { get; set; }
        public int payment_id { get; set; }
        public string payment_status { get; set; }
        public Nullable<System.DateTime> order_date { get; set; }
    
        public virtual Customer Customer { get; set; }
        public virtual payment payment { get; set; }
    }
}