using PMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebPrac.Security;

namespace WebPrac.Controllers
{
    public class ProductController : Controller
    {
        private ActionResult GetUrlToRedirect()
        {
            if (!SessionManager.IsValidUser)
            {

                return Redirect("~/User/Login");
            }
            return null;
        }
        public ActionResult ShowAll()
        {
            if (SessionManager.IsValidUser == false)
            {
                ViewBag.MSG = "Login First";
                return Redirect("~/User/Login");
            }
            String login = SessionManager.User.Login;

            var products = PMS.BAL.ProductBO.GetAllProducts(true);

            return View(products);
        }
        public ActionResult UserProfile(String pic)
        {
            if (SessionManager.IsValidUser == false)
            {
                ViewBag.MSG = "Login First";
                return Redirect("~/User/Login");
            }
            PMS.Entities.UserDTO user = new PMS.Entities.UserDTO();
            user = PMS.BAL.UserBO.GetUserByPictureName(pic);
            return View("UserProfile",user);
        }

        public ActionResult New()
        {
            var redVal = GetUrlToRedirect();
            if (redVal == null)
            {
                var dto = new ProductDTO();
                redVal =  View(dto);
            }

            return redVal;
        }

        public ActionResult Edit(int id)
        {

            var redVal = GetUrlToRedirect();
            if (redVal == null)
            {
                var prod = PMS.BAL.ProductBO.GetProductById(id);
                redVal= View("New", prod);
            }

            return redVal;
            
        }
        public ActionResult Edit2(int ProductID)
        {
            var prod = PMS.BAL.ProductBO.GetProductById(ProductID);
            return View("New", prod);
        }
        public ActionResult Delete(int id)
        {

            if (SessionManager.IsValidUser)
            {

                PMS.BAL.ProductBO.DeleteProduct(id);
                TempData["Msg"] = "Record is Deleted!";
                return RedirectToAction("ShowAll");

            }
            else
            {
                return Redirect("~/User/Login");
            }

        }
        [HttpPost]
        public ActionResult Save(ProductDTO dto)
        {
            String login = SessionManager.User.Login;
            UserDTO u = PMS.BAL.UserBO.getUserByLogin(login);
            /* if (SessionManager.IsValidUser)
             {

                 if (SessionManager.User.IsAdmin == false)
                 {
                     TempData["Message"] = "Unauthorized Access";
                     return Redirect("~/Home/NormalUser");
                 }
             }*/
            if (!SessionManager.IsValidUser)
            {
                return Redirect("~/User/Login");
            }


            var uniqueName = "";

            if (Request.Files["PictureName"] != null)
            {
                var file = Request.Files["PictureName"];
                if (file.FileName != "")
                {
                    var ext = System.IO.Path.GetExtension(file.FileName);

                    //Generate a unique name using Guid
                    uniqueName = Guid.NewGuid().ToString() + ext;

                    //Get physical path of our folder where we want to save images
                    var rootPath = Server.MapPath("~/UserPics");

                    var fileSavePath = System.IO.Path.Combine(rootPath, uniqueName);

                    // Save the uploaded file to "UploadedFiles" folder
                    file.SaveAs(fileSavePath);

                    dto.PictureName = uniqueName;
                }
            }



            if (dto.ProductID > 0)
            {
                dto.ModifiedOn = DateTime.Now;
                dto.ModifiedBy = u.UserID;
            }
            else
            {
                dto.CreatedOn = DateTime.Now;
                dto.CreatedBy = u.UserID;
            }

            PMS.BAL.ProductBO.Save(dto);

            TempData["Msg"] = "Record is saved!";

            return RedirectToAction("ShowAll");
        }

    }
}