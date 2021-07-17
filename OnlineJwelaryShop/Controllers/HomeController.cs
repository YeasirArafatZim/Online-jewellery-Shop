using OnlineJwelaryShop.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineJwelaryShop.Controllers
{
    public class HomeController : Controller
    {
        OnlineShopEntities9 db = new OnlineShopEntities9();
        string connectionString = @"Data Source = DESKTOP-GJQTPMN\ZIMSQL;  Initial Catalog =OnlineShop;  Integrated Security  = true";


        public ActionResult Index()
        {
            var data = db.categories.ToList();
            ViewBag.catagories = data;

            var a = db.products.OrderByDescending(p => p.product_id).ToList();
            ViewBag.latestProduct = a;

            var b = db.feedbacks.Where(x => x.status == "Active").OrderByDescending(x => x.feedback_id).ToList();
            ViewBag.Feedbacks = b;
            return View();
        }
        public ActionResult About()
        {
            var data = db.categories.ToList();
            ViewBag.catagories = data;
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            var data = db.categories.ToList();
            ViewBag.catagories = data;
            return View();
        }

        [HttpPost]
        public ActionResult Contact(contact a)
        {
            Message b = new Message();
            if(a.Fname!=null)
            {
                b.name = a.Fname + " "+ a.Lname;
            }
            else
            {
                TempData["Message"] = "Name Required";
                return RedirectToAction("Contact", "Home");
            }

            if (a.email != null)
            {
                b.email = a.email;
            }
            else
            {
                TempData["Message"] = "Email Required";
                return RedirectToAction("Contact", "Home");
            }
            if (a.phn != null)
            {
                b.phn = a.phn;
            }
            else
            {
                TempData["Message"] = "Phone Number Required";
                return RedirectToAction("Contact", "Home");
            }
            if (a.message != null)
            {
                b.message1 = a.message;
            }
            else
            {
                TempData["Message"] = "Message filled is empty";
                return RedirectToAction("Contact", "Home");
            }
            db.Messages.Add(b);
            db.SaveChanges();
            TempData["Success"] = "We received your Message";
            return RedirectToAction("Contact", "Home");
        }

        public ActionResult Category(int id)
        {
            DataTable dtblProductsub = new DataTable();

            var data = db.categories.ToList();
            ViewBag.catagories = data;
            using (SqlConnection sqlcon = new SqlConnection(connectionString))
            {
                sqlcon.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("Select * from sub_category where cat_id = @catid", sqlcon);
                sqlDa.SelectCommand.Parameters.AddWithValue("@catid", id);
                sqlDa.Fill(dtblProductsub);

            }
            List<submodel> cart = new List<submodel>();

            foreach (DataRow dtrows in dtblProductsub.Rows)
            {
                DataTable dtblProduct = new DataTable();

                int m = Convert.ToInt32(dtrows["sub_cat_id"].ToString());
                using (SqlConnection sqlcon = new SqlConnection(connectionString))
                {
                    sqlcon.Open();
                    SqlDataAdapter sqlDa = new SqlDataAdapter("Select * from products where sub_cat_id = @subid", sqlcon);
                    sqlDa.SelectCommand.Parameters.AddWithValue("@subid", Convert.ToInt32(dtrows["sub_cat_id"].ToString()));
                    sqlDa.Fill(dtblProduct);
                    int n = dtblProduct.Rows.Count;
                    cart.Add(new submodel()
                    {
                        subcategoryname = dtrows["name"].ToString(),
                        subproducttable = dtblProduct,
                        //  carttracnumber = cartnumber

                    });
                }
            }

            ViewBag.products = cart;
            return View();

        }
        public ActionResult aboutUs()
        {
            var data = db.categories.ToList();
            ViewBag.catagories = data;
            return View();
        }

        public ActionResult viewDetails(int id)
        {
            var data = db.categories.ToList();
            ViewBag.catagories = data;
            product u = db.products.Where(x => x.product_id == id).SingleOrDefault();
            ViewBag.product = u;
            Session["product"] = u.product_id;
            return View();
        }
        public ActionResult insertCustomOrder(string quantity)
        {
            if (Session["user"] != null)
            {
                product a = new product();
                a.product_id = Int32.Parse(Session["product"].ToString());
                product u = db.products.Where(x => x.product_id == a.product_id).SingleOrDefault();

                return RedirectToAction("Index", "Home");

            }
            else
            {
                return RedirectToAction("login", "Account");
            }
        }

        public ActionResult search(string search)
        {
            var data = db.categories.ToList();
            ViewBag.catagories = data;
            DataTable dtblProduct = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(connectionString))
            {
                sqlcon.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("Select * from products where name like '%'+@search+'%'", sqlcon);
                sqlDa.SelectCommand.Parameters.AddWithValue("@search", search);
                sqlDa.Fill(dtblProduct);
            }
            return View(dtblProduct);
        }

        public ActionResult AddtoCartView()
        {
            var data = db.categories.ToList();
            ViewBag.catagories = data;
            return View();
        }
        public ActionResult AddtocartEdit()
        {
            var data = db.categories.ToList();
            ViewBag.catagories = data;
            return View();
        }

        [HttpGet]
        public ActionResult payment()
        {
            var data = db.categories.ToList();
            ViewBag.catagories = data;

            if (Session["user"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("login", "Account");
            }
        }

        [HttpPost]
        public ActionResult payment(payment a)
        {
            payment u = new payment();
            u.amount = (int)Session["amount"];
            u.unique_id = Int32.Parse(Session["unique_id"].ToString());
            u.TxnId = a.TxnId;
            u.address = a.address;
            u.user_id = (string)Session["user"];
            u.bkash_phone_no = a.bkash_phone_no;
            u.payment_date = DateTime.Now;
            db.payments.Add(u);
            db.SaveChanges();

            payment z = db.payments.Where(x => x.user_id == u.user_id && x.unique_id == u.unique_id).SingleOrDefault();

            Order_details b = new Order_details();
            b.unique_id = Int32.Parse(Session["unique_id"].ToString());
            b.user_id = (string)Session["user"];
            b.payment_id = z.payment_id;
            b.order_date = DateTime.Now;
            b.payment_status = "Pending";
            db.Order_details.Add(b);
            db.SaveChanges();

            Session.Remove("cart");
            Session.Remove("unique_id");
            return RedirectToAction("Index", "Home");
        }

        public ActionResult categories()
        {

            var data = db.categories.ToList();
            ViewBag.catagories = data;
            return View();

        }

        public ActionResult profile()
        {
            var data = db.categories.ToList();
            ViewBag.catagories = data;

            if (Session["user"] != null)
            {
                Customer a = new Customer();
                a.user_id = (String)Session["user"];
                Customer u = db.Customers.Where(x => x.user_id == a.user_id).SingleOrDefault();
                TempData["Name"] = u.name;
                TempData["Email"] = u.email;
                TempData["Phn"] = u.PhnNo;
                TempData["Address"] = u.address;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        public int IncCartNumber()
        {
            int carttracid = 1;
            int cartnumber = 0;
            int prevQuantity = 1;
            //     int ProductId = Convert.ToInt32(Request.QueryString["id"]);
            DataTable dtblProduct = new DataTable();
            DataTable dtblProductcart = new DataTable();

            using (SqlConnection sqlcon = new SqlConnection(connectionString))
            {

                sqlcon.Open();
                // SqlCommand sqlCmdgetcart = new SqlCommand(query, sqlcon);
                string query = $"Select  * from Carttrac  where carttracid = @carttracid";
                SqlDataAdapter sqlDacart = new SqlDataAdapter(query, sqlcon);
                sqlDacart.SelectCommand.Parameters.AddWithValue("@carttracid", carttracid);
                sqlDacart.Fill(dtblProductcart);
                if (dtblProductcart.Rows.Count == 1)
                {
                    cartnumber = Convert.ToInt32(dtblProductcart.Rows[0][1]);
                    //  cartnumber = cartnumber + 1;
                }

                string cartupdatequery = "UPDATE Carttrac SET  cartnumber= @cartnumber where carttracid = @carttracid ";
                SqlCommand sqlCmdupdatecart = new SqlCommand(cartupdatequery, sqlcon);
                sqlCmdupdatecart.Parameters.AddWithValue("@carttracid", carttracid);
                sqlCmdupdatecart.Parameters.AddWithValue("@cartnumber", cartnumber);
                sqlCmdupdatecart.ExecuteNonQuery();

            }

            return cartnumber;

        }

        public void UpdateCartnum(int cartnumber)
        {
            cartnumber = cartnumber + 1;
            int carttracid = 1;
            DataTable dtblProduct = new DataTable();
            DataTable dtblProductcart = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(connectionString))
            {
                sqlcon.Open();
                //SqlCommand sqlCmdupdatecart= new SqlCommand(query, sqlcon);
                string cartupdatequery = "UPDATE Carttrac SET  cartnumber= @cartnumber where carttracid = @carttracid ";
                SqlCommand sqlCmdupdatecart = new SqlCommand(cartupdatequery, sqlcon);
                sqlCmdupdatecart.Parameters.AddWithValue("@carttracid", carttracid);
                sqlCmdupdatecart.Parameters.AddWithValue("@cartnumber", cartnumber);
                sqlCmdupdatecart.ExecuteNonQuery();

            }

        }
        public void InsertIntoCart(int Quantity, DataTable dtblProduct, int num)
        {
            using (SqlConnection sqlcon = new SqlConnection(connectionString))
            {
                sqlcon.Open();
                string query = "Insert into Cart  Values(@Quantity,@Amount,@CartNumber,@ProductId) ";
                SqlCommand sqlCmd = new SqlCommand(query, sqlcon);
                sqlCmd.Parameters.AddWithValue("@CartNumber", num);
                sqlCmd.Parameters.AddWithValue("@Amount", dtblProduct.Rows[0][2]);

                sqlCmd.Parameters.AddWithValue("@ProductId", dtblProduct.Rows[0][0]);
                sqlCmd.Parameters.AddWithValue("@Quantity", Quantity);

                sqlCmd.ExecuteNonQuery();
            }


        }

        public void UpdateCart(int Quantity, DataTable dtblProduct, int num)
        {
            using (SqlConnection sqlcon = new SqlConnection(connectionString))
            {
                sqlcon.Open();
                string cartupdatequery = "UPDATE cart SET  quantity= @Quantity where unique_id = @CartNumber and product_id = @ProductId  ";
                SqlCommand sqlCmdupdatecart = new SqlCommand(cartupdatequery, sqlcon);
                sqlCmdupdatecart.Parameters.AddWithValue("@ProductId", dtblProduct.Rows[0][0]);
                sqlCmdupdatecart.Parameters.AddWithValue("@Quantity", Quantity);
                sqlCmdupdatecart.Parameters.AddWithValue("@CartNumber", num);
                sqlCmdupdatecart.ExecuteNonQuery();

            }

        }
        //   int totalcart = 1;

        public ActionResult AddtoCart(int id)
        {
            int carttracid = 1;
            int cartnumber = 1;
            bool flag = true;
            int prevQuantity = 1;
            DataTable dtblProduct = new DataTable();
            DataTable dtblProductcart = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(connectionString))
            {
                sqlcon.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("Select * from products where product_id = @ProductId", sqlcon);
                sqlDa.SelectCommand.Parameters.AddWithValue("@ProductId", id);
                sqlDa.Fill(dtblProduct);
            }
            if (Session["cart"] == null)
            {
                cartnumber = IncCartNumber();
                MyCartnumber nm = new MyCartnumber();
                Session["cartNumber"] = cartnumber.ToString();
                Session["unique_id"] = cartnumber.ToString();
                UpdateCartnum(cartnumber);
                List<item> cart = new List<item>();
                cart.Add(new item()
                {
                    Quantity = 1,
                    ProductCartTable = dtblProduct,
                    carttracnumber = cartnumber,
                    totalcart = 1
                });
                InsertIntoCart(1, dtblProduct, cartnumber);
                Session["cart"] = cart;

            }
            else
            {
                int sendCardnumber = IncCartNumber() - 1;


                List<item> cart = (List<item>)Session["cart"];
                DataRow dtrow = dtblProduct.Rows[0];
                foreach (var item in cart.ToList())
                {
                    int Pid = Convert.ToInt32(item.ProductCartTable.Rows[0][0]);
                    if (Pid == id)
                    {
                        flag = false;
                        prevQuantity = item.Quantity + 1;
                        cart.Remove(item);
                        UpdateCart(prevQuantity, dtblProduct, sendCardnumber);
                        break;
                    }
                }
                if (flag)
                {
                    InsertIntoCart(prevQuantity, dtblProduct, sendCardnumber);
                }
                cart.Add(new item()
                {
                    Quantity = prevQuantity,
                    ProductCartTable = dtblProduct,
                    carttracnumber = IncCartNumber() - 1,
                    //totalcart = totalcart + 1
                });

                Session["cart"] = cart;

            }
            return RedirectToAction("Index");

        }
        public ActionResult RemoveFromCart(int id)
        {
            int sendCardnumber = IncCartNumber() - 1;

            using (SqlConnection sqlcon = new SqlConnection(connectionString))
            {
                sqlcon.Open();
                string cartupdatequery = "Delete From cart where unique_id=@CartNumber and  product_id=@ProductId ";
                SqlCommand sqlCmdupdatecart = new SqlCommand(cartupdatequery, sqlcon);
                sqlCmdupdatecart.Parameters.AddWithValue("@CartNumber", sendCardnumber);
                sqlCmdupdatecart.Parameters.AddWithValue("@ProductId", id);
                sqlCmdupdatecart.ExecuteNonQuery();

            }

            List<item> cart = (List<item>)Session["cart"];
            foreach (var item in cart)
            {
                int Pid = Convert.ToInt32(item.ProductCartTable.Rows[0][0]);
                if (Pid == id)
                {
                    cart.Remove(item);
                    break;
                }
            }
            Session["cart"] = cart;
            return RedirectToAction("Index");
        }


        public ActionResult AddToCartinc(int id)
        {
            int carttracid = 1;
            int cartnumber = 1;
            bool flag = true;
            int prevQuantity = 1;
            DataTable dtblProduct = new DataTable();
            DataTable dtblProductcart = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(connectionString))
            {
                sqlcon.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("Select * from products where product_id = @ProductId", sqlcon);
                sqlDa.SelectCommand.Parameters.AddWithValue("@ProductId", id);
                sqlDa.Fill(dtblProduct);
            }
            if (Session["cart"] == null)
            {
                cartnumber = IncCartNumber();
                MyCartnumber nm = new MyCartnumber();
                Session["cartNumber"] = cartnumber.ToString();
                UpdateCartnum(cartnumber);
                List<item> cart = new List<item>();
                cart.Add(new item()
                {
                    Quantity = 1,
                    ProductCartTable = dtblProduct,
                    carttracnumber = cartnumber,
                    totalcart = 1
                });
                InsertIntoCart(1, dtblProduct, cartnumber);
                Session["cart"] = cart;

            }
            else
            {
                int sendCardnumber = IncCartNumber() - 1;


                List<item> cart = (List<item>)Session["cart"];
                DataRow dtrow = dtblProduct.Rows[0];
                foreach (var item in cart.ToList())
                {
                    int Pid = Convert.ToInt32(item.ProductCartTable.Rows[0][0]);
                    if (Pid == id)
                    {
                        flag = false;
                        prevQuantity = item.Quantity + 1;
                        cart.Remove(item);
                        UpdateCart(prevQuantity, dtblProduct, sendCardnumber);
                        break;
                    }
                }
                if (flag)
                {
                    InsertIntoCart(prevQuantity, dtblProduct, sendCardnumber);
                }
                cart.Add(new item()
                {
                    Quantity = prevQuantity,
                    ProductCartTable = dtblProduct,
                    carttracnumber = IncCartNumber() - 1,
                    //totalcart = totalcart + 1
                });

                Session["cart"] = cart;

            }
            return Redirect("Addtocartedit");
        }


        public ActionResult DecreaseQty(int id)
        {
            int ProductId = Convert.ToInt32(Request.QueryString["id"]);
            DataTable dtblProduct = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(connectionString))
            {
                sqlcon.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("Select * from products where product_id = @search", sqlcon);
                sqlDa.SelectCommand.Parameters.AddWithValue("@search", id);
                sqlDa.Fill(dtblProduct);
            }
            if (Session["cart"] != null)
            {
                List<item> cart = (List<item>)Session["cart"];
                foreach (var item in cart)
                {
                    int pid = Convert.ToInt32(item.ProductCartTable.Rows[0][0]);
                    if (pid == id)
                    {
                        int prevQty = item.Quantity;
                        int sendCardnumber = IncCartNumber() - 1;

                        UpdateCart(prevQty - 1, dtblProduct, sendCardnumber);
                        if (prevQty > 0)
                        {
                            cart.Remove(item);
                            cart.Add(new item()
                            {
                                ProductCartTable = dtblProduct,
                                Quantity = prevQty - 1
                            });
                        }

                        break;
                    }
                }
                Session["cart"] = cart;
            }
            return Redirect("Addtocartedit");
        }

        public ActionResult history()
        {
            var data = db.categories.ToList();
            ViewBag.catagories = data;

            if (Session["user"] != null)
            {
                string id = Session["user"].ToString();
                var a = db.Order_details.Where(p => p.user_id == id).OrderByDescending(x => x.order_id).ToList();
                ViewBag.history = a;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult OrderHistory(int id)
        {
            var data = db.categories.ToList();
            ViewBag.catagories = data;

            if (Session["user"] != null)
            {
                Order_details u = db.Order_details.Where(x => x.order_id == id).SingleOrDefault();
                ViewBag.Order = u;
                Session["OrderId"] = u.order_id;


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
                return RedirectToAction("login", "Home");
            }
        }
        public ActionResult feedback()
        {
            var data = db.categories.ToList();
            ViewBag.catagories = data;
            return View();
        }

        [HttpPost]
        public ActionResult feedback(feedback a)
        {
            feedback b = new feedback();
            b.feedback1 = a.feedback1;
            string id = Session["user"].ToString();
            b.user_id = id;
            Customer c = db.Customers.Where(x => x.user_id == id).SingleOrDefault();
            b.email = c.email;
            b.name = c.name;
            string status = "Pending";
            b.status = status;
            db.feedbacks.Add(b);
            db.SaveChanges();
            TempData["Success"] = "We Welcome Your Feedback & Thoughts";
            Session["feedback"] = id;
            return RedirectToAction("feedback", "Home");
        }
    }
}