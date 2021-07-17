using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineJwelaryShop.Models
{
    public partial class Registration
    {
        public string name { get; set; }
        public string user_id { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string PhnNo { get; set; }
        public string password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}