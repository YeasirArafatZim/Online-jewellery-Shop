using OnlineJwelaryShop.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineJwelaryShop.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin

        OnlineShopEntities9 db = new OnlineShopEntities9();
        string connectionString = @"Data Source = DESKTOP-GJQTPMN\ZIMSQL;  Initial Catalog =OnlineShop;  Integrated Security  = true";
        public ActionResult Index()
        {
            if (Session["admin"] == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("AddCategory", "Admin");
            }
        }

        [HttpGet]
        public ActionResult AddCategory()
        {
            if (Session["admin"] != null)
            {
                var data = db.categories.ToList();
                ViewBag.catagories = data;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }

        }

        [HttpPost]
        public ActionResult login(admin a)
        {
            admin u = db.admins.Where(x => x.admin_id == a.admin_id && x.password == a.password).SingleOrDefault();
            if (u != null)
            {
                Session["admin"] = u.admin_id;
                return RedirectToAction("OrderDetails", "Admin");
            }
            else
            {
                TempData["Valid"] = "Invalid Username Or Password";
                return RedirectToAction("Index", "Admin");
            }
        }

        public ActionResult logout()
        {
            Session.Remove("admin");
            return RedirectToAction("Index", "Admin");
        }

        [HttpPost]
        public ActionResult AddCategory(category cat, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                string path = Path.Combine(Server.MapPath("~/Content/images/category"), Path.GetFileName(file.FileName));
                file.SaveAs(path);
                db.categories.Add(new category
                {
                    name = cat.name,
                    img_name = "~/Content/images/category/" + file.FileName
                });
                db.SaveChanges();

            }
            return RedirectToAction("AddCategory", "Admin");
        }



        [HttpGet]
        public ActionResult AddSubcategory()
        {
            if (Session["admin"] != null)
            {
                var items = db.categories.ToList();

                if (items != null)
                {
                    ViewBag.data = items;
                }

                var query = from s in db.sub_category
                            join c in db.categories on s.cat_id equals c.cat_id
                            select new SubCategoryWithCategory { Sub_Category = s, Category = c };
                var model = query.ToList();
                ViewBag.SubWithCat = model;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
        }

        [HttpPost]
        public ActionResult AddSubcategory(sub_category a)
        {
            sub_category s = new sub_category();
            s.cat_id = a.cat_id;
            s.name = a.name;
            db.sub_category.Add(s);
            db.SaveChanges();
            return RedirectToAction("AddSubcategory", "Admin");
        }

        [HttpGet]
        public ActionResult AddProduct()
        {
            if (Session["admin"] != null)
            {
                int id = (int)Session["cat"];
                var q = db.sub_category.Where(x => x.cat_id == id);
                var items = q.ToList();

                if (items != null)
                {
                    ViewBag.data = items;
                }

                var query = from p in db.products
                            join s in db.sub_category.Where(x => x.cat_id == id) on p.sub_cat_id equals s.sub_cat_id
                            select new ProductWithSubCategory { Sub_Category = s, Product = p };
                var model = query.ToList();
                ViewBag.ProWithSub = model;
                return View();

            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
        }

        [HttpPost]
        public ActionResult AddProduct(product a, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                string path = Path.Combine(Server.MapPath("~/Content/images/product"), Path.GetFileName(file.FileName));
                file.SaveAs(path);
                db.products.Add(new product
                {
                    name = a.name,
                    price = a.price,
                    sub_cat_id = a.sub_cat_id,
                    img_name = "~/Content/images/product/" + file.FileName
                });
                db.SaveChanges();

            }

            return RedirectToAction("AddProduct", "Admin");
        }


        [HttpGet]
        public ActionResult cat()
        {
            if (Session["admin"] != null)
            {
                var items = db.categories.ToList();

                if (items != null)
                {
                    ViewBag.data = items;
                }

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
        }

        [HttpPost]
        public ActionResult cat(category a)
        {
            category u = new category();
            u.cat_id = a.cat_id;
            Session["cat"] = u.cat_id;
            return RedirectToAction("AddProduct", "Admin");
        }

        public ActionResult deleteCategory(int id)
        {
            DataTable subcat = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(connectionString))
            {
                sqlcon.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("Select sub_cat_id from sub_category where cat_id = @cat", sqlcon);
                sqlDa.SelectCommand.Parameters.AddWithValue("@cat", id);
                sqlDa.Fill(subcat);
            }

            if (subcat.Rows.Count == 0)
            {
                using (SqlConnection sqlcon = new SqlConnection(connectionString))
                {
                    sqlcon.Open();

                    string query = "Delete from category where cat_id = @id";
                    SqlCommand sqlCmd = new SqlCommand(query, sqlcon);
                    sqlCmd.Parameters.AddWithValue("@id", id);
                    sqlCmd.ExecuteNonQuery();
                }
                return RedirectToAction("AddCategory", "Admin");
            }
            else
            {
                TempData["Message"] = "Delete Sub Categories first";
                return RedirectToAction("AddCategory", "Admin");
            }
        }

        public ActionResult deleteSubCategory(int id)
        {
            DataTable subcat = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(connectionString))
            {
                sqlcon.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("Select product_id from products where sub_cat_id = @pro", sqlcon);
                sqlDa.SelectCommand.Parameters.AddWithValue("@pro", id);
                sqlDa.Fill(subcat);
            }

            if (subcat.Rows.Count == 0)
            {
                using (SqlConnection sqlcon = new SqlConnection(connectionString))
                {
                    sqlcon.Open();

                    string query = "Delete from sub_category where sub_cat_id = @id";
                    SqlCommand sqlCmd = new SqlCommand(query, sqlcon);
                    sqlCmd.Parameters.AddWithValue("@id", id);
                    sqlCmd.ExecuteNonQuery();
                }
                return RedirectToAction("AddSubcategory", "Admin");
            }
            else
            {
                TempData["Message"] = "Delete Products first";
                return RedirectToAction("AddSubcategory", "Admin");
            }
        }

        public ActionResult deleteProduct(int id)
        {
            using (SqlConnection sqlcon = new SqlConnection(connectionString))
            {
                sqlcon.Open();

                string query = "Delete from products where product_id = @id";
                SqlCommand sqlCmd = new SqlCommand(query, sqlcon);
                sqlCmd.Parameters.AddWithValue("@id", id);
                sqlCmd.ExecuteNonQuery();
            }
            return RedirectToAction("AddProduct", "Admin");
        }

        public ActionResult OrderDetails()
        {
            if(Session["admin"]!=null)
            {
                var data = db.Order_details.OrderByDescending(x => x.order_id).ToList();
                ViewBag.orders = data;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
        }

        public ActionResult OrderDetailsView(int id)
        {
            if (Session["admin"] != null)
            {
                Order_details u = db.Order_details.Where(x => x.order_id == id).SingleOrDefault();
                ViewBag.Order = u;
                Session["OrderId"] = u.order_id; 
                Customer a = db.Customers.Where(x => x.user_id == u.user_id).SingleOrDefault();
                ViewBag.userData = a;
                payment c = db.payments.Where(x => x.unique_id == u.unique_id).SingleOrDefault();
                ViewBag.pay = c;
                DataTable cart = new DataTable();
                using (SqlConnection sqlcon = new SqlConnection(connectionString))
                {
                    sqlcon.Open();
                    SqlDataAdapter sqlDa = new SqlDataAdapter("Select * from cart where unique_id = @unique", sqlcon);
                    sqlDa.SelectCommand.Parameters.AddWithValue("@unique", u.unique_id);
                    sqlDa.Fill(cart);
                }
                List<CartProduct> pro = new List<CartProduct>();

                foreach (DataRow dtrows in cart.Rows)
                {
                    DataTable p = new DataTable();

                    int m = Convert.ToInt32(dtrows["cart_id"].ToString());
                    using (SqlConnection sqlcon = new SqlConnection(connectionString))
                    {
                        sqlcon.Open();
                        int n = Convert.ToInt32(dtrows["product_id"].ToString());
                        SqlDataAdapter sqlDa = new SqlDataAdapter("Select * from products where product_id = @proid", sqlcon);
                        sqlDa.SelectCommand.Parameters.AddWithValue("@proid", Convert.ToInt32(dtrows["product_id"].ToString()));
                        sqlDa.Fill(p);

                        pro.Add(new CartProduct()
                        {
                            ProductTable = p,
                            Quantity = Int32.Parse(dtrows["quantity"].ToString())                   
                            //  carttracnumber = cartnumber
                        });
                    }
                }
                ViewBag.product = pro;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
        }

        public ActionResult OrderStatus(string status)
        {
            string a = status;
            int orderid = Int32.Parse(Session["OrderId"].ToString());
            using (SqlConnection sqlcon = new SqlConnection(connectionString))
            {
                sqlcon.Open();

                string query = "update Order_details set payment_status =@Status where order_id = @id";
                SqlCommand sqlCmd = new SqlCommand(query, sqlcon);
                sqlCmd.Parameters.AddWithValue("@id", orderid);
                sqlCmd.Parameters.AddWithValue("@Status", a);
                sqlCmd.ExecuteNonQuery();
            }
            return RedirectToAction("OrderDetails", "Admin");
        }

        public ActionResult viewDetails(int id)
        {

            product u = db.products.Where(x => x.product_id == id).SingleOrDefault();
            ViewBag.product = u;
            Session["product"] = u.product_id;
            return View();
        }

        public ActionResult RegistrationView()
        {
            return View();
        }
        public ActionResult RegisteredCustomer()
        {
            if (Session["admin"] != null)
            {
                var customers = db.Customers.ToList();

                if (customers != null)
                {
                    ViewBag.customers = customers;
                }

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
        }

        public ActionResult deleteCustomers(string id)
        {

            if (Session["admin"] != null)
            {
                using (SqlConnection sqlcon = new SqlConnection(connectionString))
                {
                    sqlcon.Open();

                    string query = "Delete from Customer where user_id = @id";
                    SqlCommand sqlCmd = new SqlCommand(query, sqlcon);
                    sqlCmd.Parameters.AddWithValue("@id", id);
                    sqlCmd.ExecuteNonQuery();
                }
                return RedirectToAction("RegisteredCustomer", "Admin");
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
        }

        public ActionResult message()
        {
            if (Session["admin"] != null)
            {
                var m = db.Messages.OrderByDescending(x=>x.message_id).ToList();
                ViewBag.msg = m;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
        }

        public ActionResult deleteMessage(string id)
        {

            if (Session["admin"] != null)
            {
                using (SqlConnection sqlcon = new SqlConnection(connectionString))
                {
                    sqlcon.Open();

                    string query = "Delete from Messages where message_id = @id";
                    SqlCommand sqlCmd = new SqlCommand(query, sqlcon);
                    sqlCmd.Parameters.AddWithValue("@id", id);
                    sqlCmd.ExecuteNonQuery();
                }
                return RedirectToAction("message", "Admin");
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
        }

        public ActionResult feedback()
        {
            if (Session["admin"] != null)
            {
                var data = db.feedbacks.OrderByDescending(x => x.feedback_id).ToList();
                ViewBag.feed = data;
                return View();
            }
            else
                return RedirectToAction("Index", "Admin");
        }

        public ActionResult feedbackStatus(string id)
        {
            feedback b = new feedback();
            b = db.feedbacks.Where(x => x.user_id == id).SingleOrDefault();
                using (SqlConnection sqlcon = new SqlConnection(connectionString))
                {
                    sqlcon.Open();

                    string query = "Update feedback set status = @stat where user_id =@id";
                    SqlCommand sqlCmd = new SqlCommand(query, sqlcon);
                    sqlCmd.Parameters.AddWithValue("@id", id);
                    if (b.status == "Pending")
                    {
                        sqlCmd.Parameters.AddWithValue("@stat", "Active");
                    }
                    else
                        {
                            sqlCmd.Parameters.AddWithValue("@stat", "Pending");
                        }
                    sqlCmd.ExecuteNonQuery();
                }
                return RedirectToAction("feedback", "Admin");
        }

    }

}
