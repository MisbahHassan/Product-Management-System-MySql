using PMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using WebPrac.Security;

namespace WebPrac.Controllers
{
    public class UserController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(String login, String password)
        {
            Session["Login"] = login;
            var obj = PMS.BAL.UserBO.ValidateUser(login, password);
            if (obj != null)
            {
                Session["user"] = obj;
                if (obj.IsAdmin == true)
                    return Redirect("~/Home/Admin");
                else
                    return Redirect("~/Home/NormalUser");
            }

            ViewBag.MSG = "Invalid Login/Password";
            ViewBag.Login = login;

            return View();
        }
        public ActionResult CancelUser()
        {
            return View("Login");
        }

        [HttpGet]
        public ActionResult Register()
        {
            
            return View("SignUp");
        }
        public ActionResult EditProfile()
        {
            if (SessionManager.IsValidUser == false)
            {
                ViewBag.MSG = "Login First";
                return Redirect("~/User/Login");
            }
            string login = Session["login"].ToString();
            PMS.Entities.UserDTO userObj = new UserDTO();
            userObj = PMS.BAL.UserBO.getUserByLogin(login);
            return View("EditProfile",userObj);
        }
        [HttpPost]
        public ActionResult SaveEditProfile(UserDTO Obj)
        {
            /* string login = Session["login"].ToString();
             PMS.Entities.UserDTO userObj = new UserDTO();
             userObj = PMS.BAL.UserBO.getUserByLogin(login);
            int userID = Convert.ToInt32(Session["UserID"]);*/

            Obj.UserID = SessionManager.User.UserID;
            PMS.Entities.UserDTO user = new UserDTO();
            user = PMS.BAL.UserBO.GetUserById(Obj.UserID);
            
            if (!user.Login.Equals(Obj.Login) || !user.Password.Equals(Obj.Password))
            {
                Response.Write("<script>alert('Not Allowed to Change Password Or Login')</script>");
                return View("EditProfile", user);
            }
            if (!Obj.Email.Equals(user.Email))
            {
                var ob = PMS.BAL.UserBO.GetUserByEmail(Obj.Email);
                if (ob != null)
                {
                    Response.Write("<script>alert('Login or Email Already Exists')</script>");
                    return View("EditProfile", user);
                }
            }
            var image = Request.Files["PictureName"];
            var uniqueName = "";

            if (Request.Files["PictureName"] != null)
            {
                if (image.FileName != null)
                {
                    var extension = System.IO.Path.GetExtension(image.FileName);
                    uniqueName = Guid.NewGuid().ToString() + extension;
                    var rootPath = Server.MapPath("~/UserPics");
                    var saveFilePath = System.IO.Path.Combine(rootPath, uniqueName);
                    image.SaveAs(saveFilePath);
                }
            }
            Obj.PictureName = uniqueName;
            int n = PMS.BAL.UserBO.UpdateUser(Obj);
            if (n > 0)
            {
                TempData["Message"] = "Updated Successfully";
                Response.Write("<script>alert('Updated Successfully')</script>");
                SessionManager.User.Name = Obj.Name;
                return Redirect("~/Home/NormalUser");
            }
            else
            {
                TempData["Message"] = "Some Problem Occurred";
                Response.Write("<script>alert('Some Problem Occurred')</script>");
                return View("EditProfile", user);
            }
        }
        [HttpPost]
        public ActionResult Save(UserDTO dto)
        {
            //User Save Logic
            dto.IsAdmin = false;
            var image = Request.Files["PictureName"];
            var uniqueName = "";

            if (Request.Files["PictureName"] != null)
            {
                if (image.FileName != null)
                {
                    var extension = System.IO.Path.GetExtension(image.FileName);
                    uniqueName = Guid.NewGuid().ToString() + extension;
                    var rootPath = Server.MapPath("~/UserPics");
                    var saveFilePath = System.IO.Path.Combine(rootPath, uniqueName);
                    image.SaveAs(saveFilePath);
                }
            }
            dto.PictureName = uniqueName;
            String email = Request["Email"];
            String login = dto.Login;
            var ob = PMS.BAL.UserBO.isUserExist(login,email);
            if (ob != null)
            {
                Response.Write("<script>alert('Login or Email Already Exists')</script>");
                return View("SignUp");
            }
            else
            {
                var obj = PMS.BAL.UserBO.Save(dto);
                if (obj > 0)
                {
                    Response.Write("<script>alert('Successfully Signed Up')</script>");
                    return View("Login");
                }
                else
                {
                    Response.Write("<script>alert('Problem Occured')</script>");
                    return View("SignUp");
                }
            }
            
        }
        public ActionResult ResetCode(string code)
        {
            if (code == "")
            {
                Response.Write("<script>alert('Fill The Field First')</script>");
                return View("ResetCode");
            }
            else
            {
                var c = Session["ResetCode"];
                if (c.Equals(code))
                {
                    return View("NewPassword");
                }
                else
                {
                    Response.Write("<script>alert('Invalid Code')</script>");
                    return View("ResetCode");
                }
            }          
        }
        public ActionResult UpdatePassword(String password)
        {
            int value = Convert.ToInt32(Session["UpdatePassword"]);
            UserDTO u = new UserDTO();
            if (value == 0)
            {
                int userID = SessionManager.User.UserID;
                u = PMS.BAL.UserBO.GetUserById(userID);
            }
            else
            {
                String Email =Session["Email"].ToString();
                u = PMS.BAL.UserBO.GetUserByEmail(Email);
            }
            u.Password = password;
            int l=PMS.BAL.UserBO.UpdatePassword(u);
            if (l > 0)
            {
                TempData["Message"] = "Password Updated";
                Response.Write("<script>alert('Password Updated')</script>");
                return Redirect("~/Home/NormalUser");
            }
            else
            {
                TempData["Message"] = "Problem Occured";
                Response.Write("<script>alert('Some Problem Occured')</script>");
                return View("New Password");
            }
        }
        public ActionResult ChangePassword()
        {
            if (SessionManager.IsValidUser == false || SessionManager.User.IsAdmin)
            {
                ViewBag.MSG = "Login First";
                return Redirect("~/User/Login");
            }
            Session["UpdatePassword"] = 0;
            return View("NewPassword");
        }
        public ActionResult ForgetPassword()
        {
            Session["UpdatePassword"] = 1;
            return View("ForgetPassword");
        }
        [HttpPost]
        public ActionResult EmailEnter(String email)
        {
            var ob = PMS.BAL.UserBO.GetUserByEmail(email);
            if (ob == null)
            {
                Response.Write("<script>alert('Email not Exists')</script>");
                return View("ForgetPassword");
            }
            Session["Email"] = email;
            Random rnd = new Random();
            int num = rnd.Next(1, 1000);
            string code = Convert.ToString(num);
            if (PMS.BAL.UserBO.sendEmail(email, "Reset Password Code", code))
            {
                Session["ResetCode"] = code;
                Session["Email"] = email;
                return View("ResetCode");
            }
            else
            {
                Response.Write("<script>alert('Email Not Exists')</script>");
                return View("ChangePassword");
            }
        }
[HttpGet]
        public ActionResult Logout()
        {
            SessionManager.ClearSession();
            return RedirectToAction("Login");
        }


        [HttpGet]
        public ActionResult Login2()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ValidateUser(String login, String password)
        {

            Object data = null;

            try
            {
                var url = "";
                var flag = false;

                var obj = PMS.BAL.UserBO.ValidateUser(login, password);
                if (obj != null)
                {
                    flag = true;
                    SessionManager.User = obj;

                    if (obj.IsAdmin == true)
                        url = Url.Content("~/Home/Admin");
                    else
                        url = Url.Content("~/Home/NormalUser");
                }

                data = new
                {
                    valid = flag,
                    urlToRedirect = url
                };
            }
            catch (Exception)
            {
                data = new
                {
                    valid = false,
                    urlToRedirect = ""
                };
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }
	}
}