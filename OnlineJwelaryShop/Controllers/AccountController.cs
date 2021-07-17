using OnlineJwelaryShop.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace OnlineJwelaryShop.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        OnlineShopEntities9 db = new OnlineShopEntities9();
        string connectionString = @"Data Source = DESKTOP-GJQTPMN\ZIMSQL;  Initial Catalog =OnlineShop;  Integrated Security  = true";

        [HttpGet]
        public ActionResult login()
        {
            return View();
        }

        public static string EncMD5(string password)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            UTF8Encoding encoder = new UTF8Encoding();
            Byte[] originalBytes = encoder.GetBytes(password);
            Byte[] encodedBytes = md5.ComputeHash(originalBytes);
            password = BitConverter.ToString(encodedBytes).Replace("-", " ");
            var result = password.ToLower();
            return result;
        }

        public ActionResult logout()
        {
            Session.Remove("user");
            Session.Remove("cart");
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult login(UserLogin user)
        {
            string pass = EncMD5(user.password);
            Customer u = db.Customers.Where(x => x.user_id == user.user_id && x.password == pass).SingleOrDefault();
            if (u != null)
            {
                
                Session["user"] = u.user_id;
                DataTable dtbl = new DataTable();
                using (SqlConnection sqlcon = new SqlConnection(connectionString))
                {
                    sqlcon.Open();
                    SqlDataAdapter sqlDa = new SqlDataAdapter("Select * from feedback where user_id = @id", sqlcon);
                    sqlDa.SelectCommand.Parameters.AddWithValue("@id", Session["user"]);
                    sqlDa.Fill(dtbl);
                }
                int n = dtbl.Rows.Count;
                if (n == 1)
                {
                    Session["feedback"] = dtbl;
                }
                return RedirectToAction("Index","Home");
            }
            else
            {

                TempData["valid"] = "Invalid Username Or Password";
                return RedirectToAction("login", "Account");
            }
            
        }

        [HttpGet]
        public ActionResult register()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult register(Registration a)
        {
            Customer u = new Customer();

            if (a.user_id != null)
            {
               Customer b = db.Customers.Where(x => x.user_id == a.user_id).SingleOrDefault();
                if(b==null)
                    u.user_id = a.user_id;
                else
                {
                    TempData["Message"] = "User name already exists";
                    return RedirectToAction("register", "Account");
                }
            }
            else
            {
                TempData["Message"] = "User Name Required";
                TempData.Keep();
                return RedirectToAction("register", "Account");
            }
            if(a.name != null)
                u.name = a.name;
            else
            {
                TempData["Message"] = "Name Required";
                TempData.Keep();
                return RedirectToAction("register", "Account");
            }
            if (a.address != null)
                u.address = a.address;
            else
            {
                TempData["Message"] = "Address Required";
                TempData.Keep();
                return RedirectToAction("register", "Account");
            }
            if (a.password != null)
            {
                if(a.password == a.ConfirmPassword)
                {
                    string pass = EncMD5(a.password);
                    u.password = pass;
                }
                else
                {
                    TempData["Message"] = "Password doesn't match";
                    TempData.Keep();
                    return RedirectToAction("register", "Account");
                }
            }
            else
            {
                TempData["Message"] = "Password Required";
                TempData.Keep();
                return RedirectToAction("register", "Account");
            }
            
            if(a.PhnNo != null)
                u.PhnNo = a.PhnNo;
            else
            {
                TempData["Message"] = "Phone Number Required";
                TempData.Keep();
                return RedirectToAction("register", "Account");
            }
            if(a.email != null)
            {
                Customer b = db.Customers.Where(x => x.email == a.email).SingleOrDefault();
                if (b == null)
                    u.email = a.email;
                else
                {
                    TempData["Message"] = "Email already used";
                    return RedirectToAction("register", "Account");
                }
            }
            else
            {
                TempData["Message"] = "Email Address Required";
                TempData.Keep();
                return RedirectToAction("register", "Account");
            }

            db.Customers.Add(u);
            db.SaveChanges();


            Session["p_id"] = 1;
            TempData["username"] = u.user_id;
            TempData.Keep();
            return RedirectToAction("login","Account");
        }

        public ActionResult forget()
        {
            return View();
        }
        

    }
}